namespace Transactions.Application.Models.EnergyConsumption.Responses;

public class ChargePointEnergyConsumptionResponse
{
    public required Guid ChargePointId { get; set; }
    public required string ChargePointName { get; set; }
    public required double EnergyConsumed { get; set; }
    
    public required List<ChargePointConnectorEnergyConsumptionResponse> ConnectorsConsumption { get; set; }
}