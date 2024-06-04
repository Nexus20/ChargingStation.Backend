namespace Transactions.Application.Models.EnergyConsumption.Responses;

public class ChargePointsEnergyConsumptionResponse
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public required List<ChargePointEnergyConsumptionResponse> ChargePointsConsumption { get; set; }
}