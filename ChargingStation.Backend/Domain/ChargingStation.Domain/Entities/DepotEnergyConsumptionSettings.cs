using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class DepotEnergyConsumptionSettings : Entity, ITimeMarkable
{
    public Guid DepotId { get; set; }
    public Depot? Depot { get; set; }
    public double DepotEnergyLimit { get; set; }
    
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    public List<ChargePointEnergyConsumptionSettings>? ChargePointsLimits { get; set; }
    public List<EnergyConsumptionIntervalSettings>? Intervals { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}