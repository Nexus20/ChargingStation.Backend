using AutoMapper;
using ChargingStation.ChargingProfiles.Models.Requests.ConsumptionSettings;
using ChargingStation.ChargingProfiles.Specifications;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models.DepotEnergyConsumption;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using ChargingStation.InternalCommunication.Services.ChargePoints;
using ChargingStation.InternalCommunication.Services.Depots;

namespace ChargingStation.ChargingProfiles.Services.EnergyConsumption;

public class EnergyConsumptionSettingsService : IEnergyConsumptionSettingsService
{
    private readonly IDepotHttpService _depotHttpService;
    private readonly IChargePointHttpService _chargePointHttpService;
    private readonly IMapper _mapper;
    private readonly IRepository<DepotEnergyConsumptionSettings> _depotEnergyConsumptionSettingsRepository;
    private readonly ILogger<EnergyConsumptionSettingsService> _logger;

    public EnergyConsumptionSettingsService(IDepotHttpService depotHttpService, IChargePointHttpService chargePointHttpService, IMapper mapper, IRepository<DepotEnergyConsumptionSettings> depotEnergyConsumptionSettingsRepository, ILogger<EnergyConsumptionSettingsService> logger)
    {
        _depotHttpService = depotHttpService;
        _chargePointHttpService = chargePointHttpService;
        _mapper = mapper;
        _depotEnergyConsumptionSettingsRepository = depotEnergyConsumptionSettingsRepository;
        _logger = logger;
    }

    public async Task<Guid> SetEnergyConsumptionSettingsAsync(SetDepotEnergyConsumptionSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var depot = await _depotHttpService.GetByIdAsync(request.DepotId, cancellationToken);
        
        if(depot is null)
            throw new NotFoundException("Depot with id {request.DepotId} not found");
        
        var chargePointsIds = request.ChargePointsLimits.Select(x => x.ChargePointId).ToList();
        
        var chargePoints = await _chargePointHttpService.GetByIdAsync(chargePointsIds, cancellationToken);
        
        if(chargePoints.IsNullOrEmpty() || chargePoints.Count != chargePointsIds.Count)
            throw new NotFoundException("Some charge points not found");
        
        if(request.DepotEnergyLimit != request.ChargePointsLimits.Sum(x => x.ChargePointEnergyLimit))
            throw new BadRequestException("Depot energy limit must be equal to sum of charge points energy limits");
        
        if(request.DepotEnergyLimit != request.Intervals.Sum(x => x.EnergyLimit))
            throw new BadRequestException("Depot energy limit must be equal to sum of intervals energy limits");
        
        var conflictingSettingsSpecification = new GetDepotEnergyConsumptionConflictingSettings(request.DepotId, request.ValidFrom, request.ValidTo);
        
        var conflictingSettings = await _depotEnergyConsumptionSettingsRepository.GetAsync(conflictingSettingsSpecification, cancellationToken: cancellationToken);
        
        if(conflictingSettings.Count != 0)
            throw new BadRequestException("Conflicting settings found");
        
        var depotEnergyConsumptionSettings = _mapper.Map<DepotEnergyConsumptionSettings>(request);
        await _depotEnergyConsumptionSettingsRepository.AddAsync(depotEnergyConsumptionSettings, cancellationToken);
        await _depotEnergyConsumptionSettingsRepository.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Energy consumption settings for depot {RequestDepotId} has been set", request.DepotId);
        
        return depotEnergyConsumptionSettings.Id;
    }

    public async Task<DepotEnergyConsumptionSettingsResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var specification = new GetDepotEnergyConsumptionSettingsWithDetailsSpecification(id);
        
        var depotEnergyConsumptionSettings = await _depotEnergyConsumptionSettingsRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if(depotEnergyConsumptionSettings is null)
            throw new NotFoundException(nameof(DepotEnergyConsumptionSettings), id);
        
        var result = _mapper.Map<DepotEnergyConsumptionSettingsResponse>(depotEnergyConsumptionSettings);
        return result;
    }

    public async Task<DepotEnergyConsumptionSettingsResponse?> GetByDepotIdAsync(Guid depotId, CancellationToken cancellationToken)
    {
        var specification = new GetDepotEnergyConsumptionSettingsWithDetailsByDepotSpecification(depotId);
        
        var depotEnergyConsumptionSettings = await _depotEnergyConsumptionSettingsRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if(depotEnergyConsumptionSettings is null)
            return null;
        
        var result = _mapper.Map<DepotEnergyConsumptionSettingsResponse>(depotEnergyConsumptionSettings);
        return result;
    }

    public async Task<DepotEnergyConsumptionSettingsResponse?> GetByChargingStationIdAsync(Guid chargingStationId, CancellationToken cancellationToken = default)
    {
        var specification = new GetDepotEnergyConsumptionSettingsWithDetailsByChargingStationSpecification(chargingStationId);
        
        var depotEnergyConsumptionSettings = await _depotEnergyConsumptionSettingsRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if(depotEnergyConsumptionSettings is null)
            return null;
        
        depotEnergyConsumptionSettings.ChargePointsLimits = depotEnergyConsumptionSettings.ChargePointsLimits!
            .Where(x => x.ChargePointId == chargingStationId)
            .ToList();
        
        var result = _mapper.Map<DepotEnergyConsumptionSettingsResponse>(depotEnergyConsumptionSettings);
        return result;
    }
}