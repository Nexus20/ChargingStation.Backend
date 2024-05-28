using AutoMapper;
using ChargingStation.Common.Constants;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.OcppTags.Responses;
using ChargingStation.Common.Models.Reservations.Requests;
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

public class ReservationService : IReservationService
{
    private readonly ILogger<ReservationService> _logger;
    private readonly OcppTagGrpcClientService _ocppTagGrpcClientService;
    private readonly ChargePointGrpcClientService _chargePointGrpcClientService;
    private readonly ConnectorGrpcClientService _connectorGrpcClientService;
    private readonly IRepository<Reservation> _reservationRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public ReservationService(ILogger<ReservationService> logger,
                              OcppTagGrpcClientService ocppTagGrpcClientService,
                              ChargePointGrpcClientService chargePointGrpcClientService,
                              ConnectorGrpcClientService connectorGrpcClientService,
                              IRepository<Reservation> reservationRepository,
                              IPublishEndpoint publishEndpoint, 
                              IMapper mapper)
    {
        _logger = logger;
        _ocppTagGrpcClientService = ocppTagGrpcClientService;
        _chargePointGrpcClientService = chargePointGrpcClientService;
        _connectorGrpcClientService = connectorGrpcClientService;
        _reservationRepository = reservationRepository;
        _publishEndpoint = publishEndpoint;
        _mapper = mapper;
    }
    
    public async Task<ReservationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken);
        
        if (reservation is null)
            throw new NotFoundException($"Reservation with id {id} not found");
        
        var result = _mapper.Map<ReservationResponse>(reservation);
        return result;
    }

    public async Task<IPagedCollection<ReservationResponse>> GetAsync(GetReservationsRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetReservationsSpecification(request);
        var reservations = await _reservationRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);
        
        if (!reservations.Collection.Any())
            return PagedCollection<ReservationResponse>.Empty;
        
        var result = _mapper.Map<IPagedCollection<ReservationResponse>>(reservations);
        return result;
    }

    public async Task UseReservationAsync(UseReservationRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetReservationsSpecification(request);
        
        var reservation = await _reservationRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (reservation is null)
        {
            throw new NotFoundException($"Reservation with id {request.ReservationId} for charge point with id {request.ChargePointId} not found");
        }
        
        if (reservation.Status != ReserveNowResponseStatus.Accepted.ToString())
        {
            throw new BadRequestException($"Reservation with id {request.ReservationId} for charge point with id {request.ChargePointId} is not accepted");
        }
        
        reservation.IsUsed = true;
        reservation.Status = "Used";
        
        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync(cancellationToken);
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
            
            var conflictingReservationsSpecification = new GetConflictingReservationsSpecification(request.StartDateTime, request.ExpiryDateTime, TimeSpan.FromMinutes(30), request.ChargePointId, connector.Id);
            var conflictingReservations = await _reservationRepository.GetAsync(conflictingReservationsSpecification, cancellationToken: cancellationToken);
            
            if (conflictingReservations.Count != 0)
            {
                throw new BadRequestException("Conflicting reservation found");
            }
        }
        
        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);

        BackgroundJob.Schedule(
            () => SendReserveNowRequestAsync(reserveNowRequestId, request, ocppTag, reservation.ReservationId, reservation.Id, cancellationToken),
            request.StartDateTime
            );
        
        _logger.LogInformation("Reservation created for charge point with id {ChargePointId} and connector id {ConnectorId}", request.ChargePointId, request.ConnectorId);
    }

    /// <remarks>This method must be public, because Hangfire works only with public methods.</remarks>
    public async Task SendReserveNowRequestAsync(string reserveNowRequestId, CreateReservationRequest request, OcppTagResponse ocppTag, int reservationId, Guid reservationGuid, CancellationToken cancellationToken = default)
    {
        var reserveNowRequest = new ReserveNowRequest(request.ConnectorId, request.ExpiryDateTime, ocppTag.TagId, reservationId);
        var integrationOcppMessage = CentralSystemRequestIntegrationOcppMessage.Create(request.ChargePointId, reserveNowRequest, Ocpp16ActionTypes.ReserveNow, reserveNowRequestId, OcppProtocolVersions.Ocpp16);
        await _publishEndpoint.Publish(integrationOcppMessage, cancellationToken);
        
        var reservationEntity = await _reservationRepository.GetByIdAsync(reservationGuid, cancellationToken);
        
        if (reservationEntity is null)
        {
            _logger.LogWarning("Reservation with id {ReservationId} not found", reservationId);
            return;
        }
        
        reservationEntity.Status = "RequestSent";
        
        _reservationRepository.Update(reservationEntity);
        await _reservationRepository.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateReservationAsync(UpdateReservationRequest request, CancellationToken cancellationToken = default)
    {
        var reservationToUpdate = await _reservationRepository.GetByIdAsync(request.Id, cancellationToken);
        
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
        
        var conflictingReservationsSpecification = new GetConflictingReservationsSpecification(request.StartDateTime, request.ExpiryDateTime, TimeSpan.FromMinutes(30), reservationToUpdate.ChargePointId, connector.Id);
        var conflictingReservations = await _reservationRepository.GetAsync(conflictingReservationsSpecification, cancellationToken: cancellationToken);
        
        if (conflictingReservations.Count != 0)
        {
            throw new BadRequestException("Conflicting reservation found");
        }

        _mapper.Map(request, reservationToUpdate);
        reservationToUpdate.ConnectorId = connector.Id;
        
        _reservationRepository.Update(reservationToUpdate);
        await _reservationRepository.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Reservation with id {ReservationId} updated", request.Id);
    }

    public async Task ProcessReservationResponseAsync(ReserveNowResponse reservationResponse, string ocppMessageId, CancellationToken cancellationToken = default)
    {
        var specification = new GetReservationByRequestIdSpecification(ocppMessageId);
        var reservation = await _reservationRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (reservation is null)
        {
            _logger.LogWarning("Reservation with ocpp message id {OcppMessageId} not found", ocppMessageId);
            return;
        }

        reservation.Status = reservationResponse.Status.ToString();
        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync(cancellationToken);
        
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
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        
        if (reservation is null)
        {
            throw new NotFoundException($"Reservation with id {request.ReservationId} not found");
        }
        
        var cancelReservationRequestId = Guid.NewGuid().ToString("N");
        
        reservation.CancellationRequestId = cancelReservationRequestId;
        
        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync(cancellationToken);
        
        var cancelReservationRequest = new CancelReservationRequest(reservation.ReservationId);
        var integrationOcppMessage = CentralSystemRequestIntegrationOcppMessage.Create(reservation.ChargePointId, cancelReservationRequest, Ocpp16ActionTypes.CancelReservation, cancelReservationRequestId, OcppProtocolVersions.Ocpp16);
        await _publishEndpoint.Publish(integrationOcppMessage, cancellationToken);
        
        _logger.LogInformation("Reservation with id {ReservationId} canceled", request.ReservationId);
    }
    
    public async Task ProcessReservationCancellationResponseAsync(CancelReservationResponse cancelReservationResponse, string ocppMessageId, CancellationToken cancellationToken = default)
    {
        var specification = new GetReservationByCancellationRequestIdSpecification(ocppMessageId);
        var reservation = await _reservationRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (reservation is null)
        {
            _logger.LogWarning("Reservation with ocpp message id {OcppMessageId} not found", ocppMessageId);
            return;
        }
        
        if(cancelReservationResponse.Status == CancelReservationResponseStatus.Accepted)
        {
            reservation.IsCancelled = true;
            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync(cancellationToken);
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