using ChargingStation.Common.Models.DepotEnergyConsumption;

namespace Aggregator.Services.HttpServices.EnergyConsumptionSettings;

public interface IEnergyConsumptionSettingsHttpService
{
    Task<DepotEnergyConsumptionSettingsResponse?> GetEnergyConsumptionSettingsByDepotAsync(Guid depotId, CancellationToken cancellationToken = default);
    Task<DepotEnergyConsumptionSettingsResponse?> GetByChargingStationIdAsync(Guid chargePointId, CancellationToken cancellationToken = default);
}