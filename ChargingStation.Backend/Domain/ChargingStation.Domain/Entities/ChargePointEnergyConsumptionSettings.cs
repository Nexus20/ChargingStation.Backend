using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class ChargePointEnergyConsumptionSettings : Entity, ITimeMarkable
{
    public Guid ChargePointId { get; set; }
    public ChargePoint? ChargePoint { get; set; }
    
    public Guid DepotEnergyConsumptionSettingsId { get; set; }
    public DepotEnergyConsumptionSettings? DepotEnergyConsumptionSettings { get; set; }
    
    public double ChargePointEnergyLimit { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}