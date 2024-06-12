namespace ChargingStation.Common.Models.ConnectorEnergyConsumption;

public class ConnectorEnergyConsumptionResponse
{
    public Guid ConnectorId { get; set; }
    public double ConsumedEnergy { get; set; }
    public double Power { get; set; }
}