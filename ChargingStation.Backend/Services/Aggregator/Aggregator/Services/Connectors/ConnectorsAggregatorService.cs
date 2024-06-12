using Aggregator.Models.Responses;
using Aggregator.Services.HttpServices.Connectors;
using Aggregator.Services.HttpServices.EnergyConsumption;
using ChargingStation.Common.Models.General;

namespace Aggregator.Services.Connectors;

public class ConnectorsAggregatorService : IConnectorsAggregatorService
{
    private readonly IConnectorsHttpService _connectorsHttpService;
    private readonly IEnergyConsumptionHttpService _energyConsumptionHttpService;

    public ConnectorsAggregatorService(IConnectorsHttpService connectorsHttpService, IEnergyConsumptionHttpService energyConsumptionHttpService)
    {
        _connectorsHttpService = connectorsHttpService;
        _energyConsumptionHttpService = energyConsumptionHttpService;
    }

    public async Task<List<ConnectorAggregatedResponse>> GetByChargePointsIdsAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default)
    {
        var baseResponses = await _connectorsHttpService.GetAsync(chargePointsIds, cancellationToken);
        
        if(baseResponses.IsNullOrEmpty())
            return [];

        var connectorsIds = baseResponses.Select(x => x.Id).ToList();
        
        var connectorsConsumption = await _energyConsumptionHttpService.GetConnectorsEnergyConsumptionAsync(connectorsIds, cancellationToken);
        
        var response = new List<ConnectorAggregatedResponse>();
        
        foreach (var baseResponse in baseResponses)
        {
            var consumption = connectorsConsumption.FirstOrDefault(x => x.ConnectorId == baseResponse.Id);
            
            response.Add(new ConnectorAggregatedResponse
            {
                Id = baseResponse.Id,
                ChargePointId = baseResponse.ChargePointId,
                ConnectorId = baseResponse.ConnectorId,
                CreatedAt = baseResponse.CreatedAt,
                UpdatedAt = baseResponse.UpdatedAt,
                CurrentStatus = baseResponse.CurrentStatus,
                ChargingProfilesIds = baseResponse.ChargingProfilesIds,
                Consumption = new ConnectorConsumptionResponse
                {
                    Power = consumption?.Power ?? 0,
                    ConsumedEnergy = consumption?.ConsumedEnergy ?? 0
                }
            });
        }
        
        return response;
    }
}