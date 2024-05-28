using AutoMapper;
using ChargePoints.Application.Models.Requests;
using ChargePoints.Application.Specifications;
using ChargingStation.CacheManager;
using ChargingStation.Common.Constants;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models.ChargePoints.Requests;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ChargePoints.Application.Services;

public class ChargePointService : IChargePointService
{
    private readonly IRepository<ChargePoint> _chargePointRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ChargePointService> _logger;
    private readonly ICacheManager _cacheManager;

    public ChargePointService(IRepository<ChargePoint> chargePointRepository, IMapper mapper, IPublishEndpoint publishEndpoint, ILogger<ChargePointService> logger, ICacheManager cacheManager)
    {
        _chargePointRepository = chargePointRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _cacheManager = cacheManager;
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
    
    public async Task<List<ChargePointResponse>> GetByDepotsIdsAsync(List<Guid> depotsIds, CancellationToken cancellationToken = default)
    {
        var specification = new GetChargePointsSpecification(depotsIds);

        var chargePoints = await _chargePointRepository.GetAsync(specification, cancellationToken: cancellationToken);

        if (chargePoints.Count == 0)
            return [];

        var result = _mapper.Map<List<ChargePointResponse>>(chargePoints);
        return result;
    }

    public async Task<List<ChargePointResponse>> GetByIdsAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default)
    {
        var specification = new GetChargePointsByIdsSpecification(chargePointsIds);
        
        var chargePoints = await _chargePointRepository.GetAsync(specification, cancellationToken: cancellationToken);
        
        if (chargePoints.Count == 0)
            return [];
        
        var result = _mapper.Map<List<ChargePointResponse>>(chargePoints);
        return result;
    }

    public async Task ChangeAvailabilityAsync(ChangeChargePointAvailabilityRequest request,
        CancellationToken cancellationToken = default)
    {
        var chargePoint = await _chargePointRepository.GetByIdAsync(request.ChargePointId, cancellationToken);
        
        if (chargePoint is null)
            throw new NotFoundException(nameof(ChargePoint), request.ChargePointId);

        var changeAvailabilityRequest = new ChangeAvailabilityRequest(0, request.AvailabilityType);
        
        var integrationOcppMessage = CentralSystemRequestIntegrationOcppMessage.Create(request.ChargePointId, changeAvailabilityRequest, Ocpp16ActionTypes.ChangeAvailability, Guid.NewGuid().ToString("N"), OcppProtocolVersions.Ocpp16);
        await _cacheManager.SetAsync(integrationOcppMessage.OcppMessageId, changeAvailabilityRequest);
        
        await _publishEndpoint.Publish(integrationOcppMessage, cancellationToken);
        _logger.LogInformation("Change availability to \"{NewAvailability}\" request sent to charge point with id {ChargePointId}", request.AvailabilityType.ToString(), request.ChargePointId);
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