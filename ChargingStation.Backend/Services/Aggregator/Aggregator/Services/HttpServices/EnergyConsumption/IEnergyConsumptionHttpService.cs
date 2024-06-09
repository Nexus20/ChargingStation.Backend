using ChargingStation.Common.Models.ConnectorEnergyConsumption;

namespace Aggregator.Services.HttpServices.EnergyConsumption;

public interface IEnergyConsumptionHttpService
{
    Task<List<ConnectorEnergyConsumptionResponse>> GetConnectorsEnergyConsumptionAsync(List<Guid> connectorsIds, CancellationToken cancellationToken = default);
}