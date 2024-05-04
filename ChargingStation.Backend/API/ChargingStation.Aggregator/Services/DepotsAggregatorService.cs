using AutoMapper;
using ChargingStation.Aggregator.Models.Responses;
using ChargingStation.Aggregator.Services.ChargePoints;
using ChargingStation.Aggregator.Services.Connectors;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16.Requests.Enums;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.InternalCommunication.Services.Depots;
using ChargingStation.InternalCommunication.Services.EnergyConsumption;

namespace ChargingStation.Aggregator.Services;

public class DepotsAggregatorService : IDepotsAggregatorService
{
    private readonly IDepotHttpService _depotsHttpService;
    private readonly IChargePointsHttpService _chargePointsHttpService;
    private readonly IActiveChargePointsHttpService _activeChargePointsHttpService;
    private readonly IConnectorsHttpService _connectorsHttpService;
    private readonly IMapper _mapper;
    private readonly IEnergyConsumptionHttpService _energyConsumptionHttpService;

    public DepotsAggregatorService(IDepotHttpService depotsHttpService, IChargePointsHttpService chargePointsHttpService, IActiveChargePointsHttpService activeChargePointsHttpService, IConnectorsHttpService connectorsHttpService, IMapper mapper, IEnergyConsumptionHttpService energyConsumptionHttpService)
    {
        _depotsHttpService = depotsHttpService;
        _chargePointsHttpService = chargePointsHttpService;
        _activeChargePointsHttpService = activeChargePointsHttpService;
        _connectorsHttpService = connectorsHttpService;
        _mapper = mapper;
        _energyConsumptionHttpService = energyConsumptionHttpService;
    }

    public async Task<IPagedCollection<DepotAggregatedResponse>> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default)
    {
        var baseDepotsResponses = await _depotsHttpService.GetAsync(request, cancellationToken);
        
        if (baseDepotsResponses.IsNullOrEmpty())
            return PagedCollection<DepotAggregatedResponse>.Empty;
        
        var depots = _mapper.Map<IPagedCollection<DepotAggregatedResponse>>(baseDepotsResponses);
        
        var depotsIds = depots.Collection.Select(x => x.Id).ToList();
        
        var chargePoints = await _chargePointsHttpService.GetAsync(depotsIds, cancellationToken);
        
        if (chargePoints.IsNullOrEmpty())
            return depots;

        var activeChargePoints = await _activeChargePointsHttpService.GetActiveChargePointsAsync(cancellationToken);

        List<ConnectorResponse>? activeChargePointsConnectors = null;
        
        if (!activeChargePoints.IsNullOrEmpty())
        {
            var activeChargePointsIds = activeChargePoints.Select(x => x.ChargePointId).ToList();
            activeChargePointsConnectors = await _connectorsHttpService.GetAsync(activeChargePointsIds, cancellationToken);
        }
        
        foreach (var depot in depots.Collection)
        {
            var chargePointsByDepot = chargePoints.Where(x => x.DepotId == depot.Id).ToList();
            
            var totalChargePointsByDepot = chargePointsByDepot.Count;

            if (activeChargePoints.IsNullOrEmpty())
            {
                depot.ChargePointsStatistics.Online = 0;
            }
            else
            {
                var activeChargePointsIds = activeChargePoints.Select(x => x.ChargePointId).ToList();
                var activeChargePointsByDepot = chargePointsByDepot.Where(x => activeChargePointsIds.Contains(x.Id)).ToList();
                depot.ChargePointsStatistics.Online = activeChargePointsByDepot.Count;
                
                if (depot.ChargePointsStatistics.Online > 0 && !activeChargePointsConnectors.IsNullOrEmpty())
                {
                    var activeChargePointsIdsByDepot = activeChargePointsByDepot.Select(x => x.Id).ToList();
                    var connectorsByChargePoints = activeChargePointsConnectors.Where(x => activeChargePointsIdsByDepot.Contains(x.ChargePointId)).ToList();
                    var activeChargePointsWithFaultedConnectorsCount = connectorsByChargePoints
                        .Where(x => x.CurrentStatus != null && x.CurrentStatus.CurrentStatus == StatusNotificationRequestStatus.Faulted.ToString())
                        .Select(x => x.ChargePointId)
                        .Distinct()
                        .Count();
                    
                    depot.ChargePointsStatistics.HasErrors = activeChargePointsWithFaultedConnectorsCount;
                }
            }
            
            depot.ChargePointsStatistics.Offline = totalChargePointsByDepot - depot.ChargePointsStatistics.Online;
        } 
        
        return depots;
    }

    public async Task<DepotAggregatedDetailsResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var baseDepotResponse = await _depotsHttpService.GetByIdAsync(id, cancellationToken);
        
        if (baseDepotResponse is null)
            throw new NotFoundException($"Depot with id {id} not found");
        
        var aggregatedResponse = _mapper.Map<DepotAggregatedDetailsResponse>(baseDepotResponse);
        
        var energyConsumptionSettings = await _energyConsumptionHttpService.GetEnergyConsumptionSettingsByDepotAsync(id, cancellationToken);
        
        if (energyConsumptionSettings is not null)
            aggregatedResponse.EnergyConsumptionSettings = energyConsumptionSettings;
        
        return aggregatedResponse;
    }
}