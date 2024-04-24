using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class EnergyConsumptionIntervalSettings : Entity, ITimeMarkable
{
    public Guid DepotEnergyConsumptionSettingsId { get; set; }
    public DepotEnergyConsumptionSettings? DepotEnergyConsumptionSettings { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public double EnergyLimit { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}