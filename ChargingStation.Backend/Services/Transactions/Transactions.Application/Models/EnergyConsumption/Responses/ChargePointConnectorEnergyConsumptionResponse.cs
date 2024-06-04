namespace Transactions.Application.Models.EnergyConsumption.Responses;

public class ChargePointConnectorEnergyConsumptionResponse
{
    public required Guid ConnectorId { get; set; }
    public required int ConnectorNumber { get; set; }
    public required double EnergyConsumed { get; set; }
}