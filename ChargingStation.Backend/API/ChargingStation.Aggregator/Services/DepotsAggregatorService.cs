using ChargingStation.Aggregator.Models.Requests;
using ChargingStation.Aggregator.Models.Responses;
using ChargingStation.Aggregator.Services.ChargePoints;
using ChargingStation.Aggregator.Services.Depots;
using ChargingStation.Common.Models.General;

namespace ChargingStation.Aggregator.Services;

public class DepotsAggregatorService : IDepotsAggregatorService
{
    private readonly IDepotsHttpService _depotsHttpService;
    private readonly IChargePointsHttpService _chargePointsHttpService;
    private readonly IActiveChargePointsHttpService _activeChargePointsHttpService;

    public DepotsAggregatorService(IDepotsHttpService depotsHttpService, IChargePointsHttpService chargePointsHttpService, IActiveChargePointsHttpService activeChargePointsHttpService)
    {
        _depotsHttpService = depotsHttpService;
        _chargePointsHttpService = chargePointsHttpService;
        _activeChargePointsHttpService = activeChargePointsHttpService;
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

        foreach (var depot in depots.Collection)
        {
            var chargePointsByDepot = chargePoints.Where(x => x.DepotId == depot.Id).ToList();
            
            depot.ChargePointsStatistics.Total = chargePointsByDepot.Count;
            
            if (activeChargePoints.IsNullOrEmpty())
                continue;
            
            var activeChargePointsIds = activeChargePoints.Select(x => x.ChargePointId).ToList();
            
            depot.ChargePointsStatistics.Online = chargePointsByDepot.Count(x => activeChargePointsIds.Contains(x.Id));
        } 
        
        return depots;
    }
}