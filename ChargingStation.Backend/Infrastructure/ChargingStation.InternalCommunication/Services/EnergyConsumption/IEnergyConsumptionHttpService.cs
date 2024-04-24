using ChargingStation.Common.Models.DepotEnergyConsumption;

namespace ChargingStation.InternalCommunication.Services.EnergyConsumption;

public interface IEnergyConsumptionHttpService
{
    Task<DepotEnergyConsumptionSettingsResponse?> GetEnergyConsumptionSettingsByDepotAsync(Guid depotId);
}