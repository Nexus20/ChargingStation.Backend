using AutoMapper;
using ChargingStation.ChargePoints.Models.Requests;
using ChargingStation.ChargePoints.Models.Responses;
using ChargingStation.ChargePoints.Specifications;
using ChargingStation.Common.Constants;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using MassTransit;

namespace ChargingStation.ChargePoints.Services;

public class ChargePointService : IChargePointService
{
    private readonly IRepository<ChargePoint> _chargePointRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ChargePointService> _logger;

    public ChargePointService(IRepository<ChargePoint> chargePointRepository, IMapper mapper, IPublishEndpoint publishEndpoint, ILogger<ChargePointService> logger)
    {
        _chargePointRepository = chargePointRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<IPagedCollection<ChargePointResponse>> GetAsync(GetChargePointRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetChargePointsSpecification(request);

        var chargePoints = await _chargePointRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);

        if (!chargePoints.Collection.Any())
            return PagedCollection<ChargePointResponse>.Empty;

        var result = _mapper.Map<IPagedCollection<ChargePointResponse>>(chargePoints);
        return result;
    }

    public async Task<ChargePointResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _chargePointRepository.GetByIdAsync(id, cancellationToken);

        if (chargePoint is null)
            throw new NotFoundException(nameof(ChargePoint), id);

        var result = _mapper.Map<ChargePointResponse>(chargePoint);
        return result;
    }

    public async Task<ChargePointResponse> CreateAsync(CreateChargePointRequest createChargePointRequest, CancellationToken cancellationToken = default)
    {
        var chargePointToCreate = _mapper.Map<ChargePoint>(createChargePointRequest);
        await _chargePointRepository.AddAsync(chargePointToCreate, cancellationToken);
        await _chargePointRepository.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<ChargePointResponse>(chargePointToCreate);
        return result;
    }

    public async Task<ChargePointResponse> UpdateAsync(UpdateChargePointRequest updateChargePointRequest, CancellationToken cancellationToken = default)
    {
        var chargePointToUpdate = await _chargePointRepository.GetByIdAsync(updateChargePointRequest.Id, cancellationToken);

        if (chargePointToUpdate is null)
            throw new NotFoundException(nameof(ChargePoint), updateChargePointRequest.Id);

        _mapper.Map(updateChargePointRequest, chargePointToUpdate);
        _chargePointRepository.Update(chargePointToUpdate);
        
        await _chargePointRepository.SaveChangesAsync(cancellationToken);
        
        var result = _mapper.Map<ChargePointResponse>(chargePointToUpdate);
        return result;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chargePointToRemove = await _chargePointRepository.GetByIdAsync(id, cancellationToken);

        if (chargePointToRemove == null)
            throw new NotFoundException(nameof(ChargePoint), id);

        _chargePointRepository.Remove(chargePointToRemove);
        
        await _chargePointRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ResetAsync(ResetChargePointRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reset request received for charge point with id {ChargePointId}", request.ChargePointId);
        var chargePoint = await _chargePointRepository.GetByIdAsync(request.ChargePointId, cancellationToken);

        if (chargePoint is null)
            throw new NotFoundException(nameof(ChargePoint), request.ChargePointId);

        var resetRequest = new ResetRequest(request.ResetType);

        var integrationOcppMessage = new IntegrationOcppMessage<ResetRequest>(request.ChargePointId, resetRequest, Guid.NewGuid().ToString("N"), OcppProtocolVersions.Ocpp16);
        
        await _publishEndpoint.Publish(integrationOcppMessage, cancellationToken);
        _logger.LogInformation("Reset request sent to charge point with id {ChargePointId}", request.ChargePointId);
    }
}