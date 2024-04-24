using ChargingStation.Common.Models.Abstract;
using ChargingStation.Common.Models.DepotEnergyConsumption.Dtos;

namespace ChargingStation.Common.Models.DepotEnergyConsumption;

public class DepotEnergyConsumptionSettingsResponse : BaseResponse, ITimeMarkable
{
    public Guid DepotId { get; set; }
    public double DepotEnergyLimit { get; set; }
    
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    
    public required List<ChargePointEnergyConsumptionSettingsDto> ChargePointsLimits { get; set; }
    
    public required List<EnergyConsumptionIntervalSettingsDto> Intervals { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}