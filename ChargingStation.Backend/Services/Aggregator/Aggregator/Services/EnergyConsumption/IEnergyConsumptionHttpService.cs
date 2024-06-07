using ChargingStation.Common.Models.DepotEnergyConsumption;

namespace Aggregator.Services.EnergyConsumption;

public interface IEnergyConsumptionHttpService
{
    Task<DepotEnergyConsumptionSettingsResponse?> GetEnergyConsumptionSettingsByDepotAsync(Guid depotId, CancellationToken cancellationToken = default);
    Task<DepotEnergyConsumptionSettingsResponse?> GetByChargingStationIdAsync(Guid chargePointId, CancellationToken cancellationToken = default);
}