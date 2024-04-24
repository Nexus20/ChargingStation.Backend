namespace ChargingStation.Common.Models.DepotEnergyConsumption.Dtos;

public class ChargePointEnergyConsumptionSettingsDto
{
    public Guid ChargePointId { get; set; }
    public double ChargePointEnergyLimit { get; set; }
}