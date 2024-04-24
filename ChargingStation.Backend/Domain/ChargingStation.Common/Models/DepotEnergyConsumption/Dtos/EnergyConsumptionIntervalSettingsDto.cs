namespace ChargingStation.Common.Models.DepotEnergyConsumption.Dtos;

public class EnergyConsumptionIntervalSettingsDto
{
    public double EnergyLimit { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}