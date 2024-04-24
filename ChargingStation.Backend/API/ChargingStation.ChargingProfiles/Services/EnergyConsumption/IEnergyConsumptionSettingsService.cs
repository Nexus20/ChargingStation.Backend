using ChargingStation.ChargingProfiles.Models.Requests.ConsumptionSettings;
using ChargingStation.Common.Models.DepotEnergyConsumption;

namespace ChargingStation.ChargingProfiles.Services.EnergyConsumption;

public interface IEnergyConsumptionSettingsService
{
    Task SetEnergyConsumptionSettingsAsync(SetDepotEnergyConsumptionSettingsRequest request, CancellationToken cancellationToken = default);
    Task<DepotEnergyConsumptionSettingsResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DepotEnergyConsumptionSettingsResponse?> GetByDepotIdAsync(Guid depotId, CancellationToken cancellationToken);
}