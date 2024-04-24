using ChargingStation.Common.Models.DepotEnergyConsumption.Dtos;

namespace ChargingStation.ChargingProfiles.Models.Requests.ConsumptionSettings;

public class SetDepotEnergyConsumptionSettingsRequest
{
    public Guid DepotId { get; set; }
    public double DepotEnergyLimit { get; set; }
    
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    
    public required List<ChargePointEnergyConsumptionSettingsDto> ChargePointsLimits { get; set; }
    
    public required List<EnergyConsumptionIntervalSettingsDto> Intervals { get; set; }
}