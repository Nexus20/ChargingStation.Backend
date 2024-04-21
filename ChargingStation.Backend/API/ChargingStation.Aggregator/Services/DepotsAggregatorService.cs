using ChargingStation.Aggregator.Models.Requests;
using ChargingStation.Aggregator.Models.Responses;
using ChargingStation.Aggregator.Services.ChargePoints;
using ChargingStation.Aggregator.Services.Connectors;
using ChargingStation.Aggregator.Services.Depots;
using ChargingStation.Common.Messages_OCPP16.Requests.Enums;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.Common.Models.General;

namespace ChargingStation.Aggregator.Services;

public class DepotsAggregatorService : IDepotsAggregatorService
{
    private readonly IDepotsHttpService _depotsHttpService;
    private readonly IChargePointsHttpService _chargePointsHttpService;
    private readonly IActiveChargePointsHttpService _activeChargePointsHttpService;
    private readonly IConnectorsHttpService _connectorsHttpService;

    public DepotsAggregatorService(IDepotsHttpService depotsHttpService, IChargePointsHttpService chargePointsHttpService, IActiveChargePointsHttpService activeChargePointsHttpService, IConnectorsHttpService connectorsHttpService)
    {
        _depotsHttpService = depotsHttpService;
        _chargePointsHttpService = chargePointsHttpService;
        _activeChargePointsHttpService = activeChargePointsHttpService;
        _connectorsHttpService = connectorsHttpService;
    }

    public async Task<IPagedCollection<DepotAggregatedResponse>> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default)
    {
        var depots = await _depotsHttpService.GetAsync(request, cancellationToken);
        
        if (depots.IsNullOrEmpty())
            return PagedCollection<DepotAggregatedResponse>.Empty;
        
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
}