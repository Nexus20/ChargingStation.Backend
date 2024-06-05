using AutoMapper;
using ChargingStation.Common.Constants;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.OcppTags.Responses;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using ChargingStation.InternalCommunication.GrpcClients;
using ChargingStation.InternalCommunication.SignalRModels;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Reservations.Application.Models.Requests;
using Reservations.Application.Models.Responses;
using Reservations.Application.Specifications;

namespace Reservations.Application.Services.Reservations;

public class ReservationService : BaseReservationService, IReservationService
{
    private readonly ILogger<ReservationService> _logger;
    private readonly OcppTagGrpcClientService _ocppTagGrpcClientService;
    private readonly ChargePointGrpcClientService _chargePointGrpcClientService;
    private readonly ConnectorGrpcClientService _connectorGrpcClientService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public ReservationService(ILogger<ReservationService> logger,
                              OcppTagGrpcClientService ocppTagGrpcClientService,
                              ChargePointGrpcClientService chargePointGrpcClientService,
                              ConnectorGrpcClientService connectorGrpcClientService,
                              IRepository<Reservation> reservationRepository,
                              IPublishEndpoint publishEndpoint, 
                              IMapper mapper) : base(reservationRepository)
    {
        _logger = logger;
        _ocppTagGrpcClientService = ocppTagGrpcClientService;
        _chargePointGrpcClientService = chargePointGrpcClientService;
        _connectorGrpcClientService = connectorGrpcClientService;
        _publishEndpoint = publishEndpoint;
        _mapper = mapper;
    }
    
    public async Task<ReservationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await ReservationRepository.GetByIdAsync(id, cancellationToken);
        
        if (reservation is null)
            throw new NotFoundException($"Reservation with id {id} not found");
        
        var result = _mapper.Map<ReservationResponse>(reservation);
        return result;
    }

    public async Task<IPagedCollection<ReservationResponse>> GetAsync(GetReservationsRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetReservationsSpecification(request);
        var reservations = await ReservationRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);
        
        if (!reservations.Collection.Any())
            return PagedCollection<ReservationResponse>.Empty;
        
        var result = _mapper.Map<IPagedCollection<ReservationResponse>>(reservations);
        return result;
    }

    public async Task CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        var ocppTag = await _ocppTagGrpcClientService.GetByIdAsync(request.OcppTagId, cancellationToken);
        
        if (ocppTag is null)
            throw new NotFoundException($"OcppTag with id {request.OcppTagId} not found");
        
        var chargePoint = await _chargePointGrpcClientService.GetByIdAsync(request.ChargePointId, cancellationToken);
        
        if (chargePoint is null)
            throw new NotFoundException($"Charge point with id {request.ChargePointId} not found");
        
        var reserveNowRequestId = Guid.NewGuid().ToString("N");

        var reservation = new Reservation
        {
            ChargePointId = request.ChargePointId,
            TagId = ocppTag.Id,
            StartDateTime = request.StartDateTime,
            ExpiryDateTime = request.ExpiryDateTime,
            ReservationRequestId = reserveNowRequestId,
            Name = request.Name,
            Description = request.Description,
            Status = "Created"
        };
        
        if (request.ConnectorId != 0)
        {
            var getOrCreateConnectorRequest = new GetOrCreateConnectorRequest()
            {
                ChargePointId = request.ChargePointId,
                ConnectorId = request.ConnectorId
            };
            var connector = await _connectorGrpcClientService.GetOrCreateConnectorAsync(getOrCreateConnectorRequest, cancellationToken);
            
            if (connector is null)
            {
                throw new NotFoundException($"Connector with id {request.ConnectorId} not found");
            }
            
            reservation.ConnectorId = connector.Id;
            
            var conflictingReservations = await GetConflictingReservationsAsync(request.StartDateTime, request.ExpiryDateTime, request.ChargePointId, connector.Id, TimeSpan.FromMinutes(30), cancellationToken);
            
            if (conflictingReservations.Count != 0)
            {
                throw new BadRequestException("Conflicting reservation found");
            }
        }
        
        await ReservationRepository.AddAsync(reservation, cancellationToken);
        await ReservationRepository.SaveChangesAsync(cancellationToken);

        BackgroundJob.Schedule(
            () => SendReserveNowRequestAsync(reserveNowRequestId, request, ocppTag, reservation.ReservationId, reservation.Id, cancellationToken),
            request.StartDateTime
            );
        
        _logger.LogInformation("Reservation created for charge point with id {ChargePointId} and connector id {ConnectorId}", request.ChargePointId, request.ConnectorId);
    }

    private async Task<List<Reservation>> GetConflictingReservationsAsync(DateTime startDateTime, DateTime expiryDateTime, Guid chargePointId, Guid connectorId, TimeSpan gapBetweenReservations, CancellationToken cancellationToken = default)
    {
        var conflictingReservationsSpecification = new GetConflictingReservationsSpecification(chargePointId, connectorId);
        var reservationsWithSameConnector = await ReservationRepository.GetAsync(conflictingReservationsSpecification, cancellationToken: cancellationToken);
        
        var conflictingReservations = reservationsWithSameConnector.Where(r =>
            (r.StartDateTime >= startDateTime && startDateTime <= r.ExpiryDateTime + gapBetweenReservations) ||
            (r.ExpiryDateTime <= expiryDateTime + gapBetweenReservations && expiryDateTime >= r.StartDateTime))
            .ToList();
        
        return conflictingReservations;
    }

    /// <remarks>This method must be public, because Hangfire works only with public methods.</remarks>
    public async Task SendReserveNowRequestAsync(string reserveNowRequestId, CreateReservationRequest request, OcppTagResponse ocppTag, int reservationId, Guid reservationGuid, CancellationToken cancellationToken = default)
    {
        var reserveNowRequest = new ReserveNowRequest(request.ConnectorId, request.ExpiryDateTime, ocppTag.TagId, reservationId);
        var integrationOcppMessage = CentralSystemRequestIntegrationOcppMessage.Create(request.ChargePointId, reserveNowRequest, Ocpp16ActionTypes.ReserveNow, reserveNowRequestId, OcppProtocolVersions.Ocpp16);
        await _publishEndpoint.Publish(integrationOcppMessage, cancellationToken);
        
        var reservationEntity = await ReservationRepository.GetByIdAsync(reservationGuid, cancellationToken);
        
        if (reservationEntity is null)
        {
            _logger.LogWarning("Reservation with id {ReservationId} not found", reservationId);
            return;
        }
        
        reservationEntity.Status = "RequestSent";
        
        ReservationRepository.Update(reservationEntity);
        await ReservationRepository.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateReservationAsync(UpdateReservationRequest request, CancellationToken cancellationToken = default)
    {
        var reservationToUpdate = await ReservationRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (reservationToUpdate is null)
            throw new NotFoundException($"Reservation with id {request.Id} not found");
        
        var getOrCreateConnectorRequest = new GetOrCreateConnectorRequest
        {
            ChargePointId = request.ChargePointId,
            ConnectorId = request.ConnectorId
        };
        var connector = await _connectorGrpcClientService.GetOrCreateConnectorAsync(getOrCreateConnectorRequest, cancellationToken);
            
        if (connector is null)
        {
            throw new NotFoundException($"Connector with id {request.ConnectorId} not found");
        }
        
        var conflictingReservations = await GetConflictingReservationsAsync(request.StartDateTime, request.ExpiryDateTime, request.ChargePointId, connector.Id, TimeSpan.FromMinutes(30), cancellationToken);
        
        if (conflictingReservations.Count != 0)
        {
            throw new BadRequestException("Conflicting reservation found");
        }

        _mapper.Map(request, reservationToUpdate);
        reservationToUpdate.ConnectorId = connector.Id;
        
        ReservationRepository.Update(reservationToUpdate);
        await ReservationRepository.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Reservation with id {ReservationId} updated", request.Id);
    }

    public async Task ProcessReservationResponseAsync(ReserveNowResponse reservationResponse, string ocppMessageId, CancellationToken cancellationToken = default)
    {
        var specification = new GetReservationByRequestIdSpecification(ocppMessageId);
        var reservation = await ReservationRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (reservation is null)
        {
            _logger.LogWarning("Reservation with ocpp message id {OcppMessageId} not found", ocppMessageId);
            return;
        }

        reservation.Status = reservationResponse.Status.ToString();
        ReservationRepository.Update(reservation);
        await ReservationRepository.SaveChangesAsync(cancellationToken);
        
        var reservationConfirmedMessage = new ReservationProcessedMessage
        {
            ReservationId = reservation.Id,
            ConnectorId = reservation.ConnectorId,
            ExpiryDate = reservation.ExpiryDateTime,
            Status = reservationResponse.Status
        };
        
        var energyLimitExceededSignalRMessage = new SignalRMessage(JsonConvert.SerializeObject(reservationConfirmedMessage), nameof(ReservationProcessedMessage));
        await _publishEndpoint.Publish(energyLimitExceededSignalRMessage, cancellationToken);
        
        _logger.LogInformation("Reservation with id {ReservationId} updated with status {Status}", reservation.ReservationId, reservationResponse.Status);
    }
    
    public async Task CreateReservationCancellation(CreateReservationCancellationRequest request, CancellationToken cancellationToken = default)
    {
        var reservation = await ReservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        
        if (reservation is null)
        {
            throw new NotFoundException($"Reservation with id {request.ReservationId} not found");
        }
        
        var cancelReservationRequestId = Guid.NewGuid().ToString("N");
        
        reservation.CancellationRequestId = cancelReservationRequestId;
        
        ReservationRepository.Update(reservation);
        await ReservationRepository.SaveChangesAsync(cancellationToken);
        
        var cancelReservationRequest = new CancelReservationRequest(reservation.ReservationId);
        var integrationOcppMessage = CentralSystemRequestIntegrationOcppMessage.Create(reservation.ChargePointId, cancelReservationRequest, Ocpp16ActionTypes.CancelReservation, cancelReservationRequestId, OcppProtocolVersions.Ocpp16);
        await _publishEndpoint.Publish(integrationOcppMessage, cancellationToken);
        
        _logger.LogInformation("Reservation with id {ReservationId} canceled", request.ReservationId);
    }
    
    public async Task ProcessReservationCancellationResponseAsync(CancelReservationResponse cancelReservationResponse, string ocppMessageId, CancellationToken cancellationToken = default)
    {
        var specification = new GetReservationByCancellationRequestIdSpecification(ocppMessageId);
        var reservation = await ReservationRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (reservation is null)
        {
            _logger.LogWarning("Reservation with ocpp message id {OcppMessageId} not found", ocppMessageId);
            return;
        }
        
        if(cancelReservationResponse.Status == CancelReservationResponseStatus.Accepted)
        {
            reservation.IsCancelled = true;
            ReservationRepository.Update(reservation);
            await ReservationRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Reservation with id {ReservationId} canceled", reservation.ReservationId);
        }
        else
        {
            _logger.LogWarning("Cancellation of reservation with id {ReservationId} rejected", reservation.ReservationId);
        }
        
        var reservationConfirmedMessage = new ReservationCancellationProcessedMessage
        {
            ReservationId = reservation.Id,
            ConnectorId = reservation.ConnectorId,
            Status = cancelReservationResponse.Status
        };
        
        var energyLimitExceededSignalRMessage = new SignalRMessage(JsonConvert.SerializeObject(reservationConfirmedMessage), nameof(ReservationCancellationProcessedMessage));
        await _publishEndpoint.Publish(energyLimitExceededSignalRMessage, cancellationToken);
    }
}