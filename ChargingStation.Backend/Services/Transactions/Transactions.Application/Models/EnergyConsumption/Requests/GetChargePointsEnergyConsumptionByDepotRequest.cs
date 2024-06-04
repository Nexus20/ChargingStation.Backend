namespace Transactions.Application.Models.EnergyConsumption.Requests;

public class GetChargePointsEnergyConsumptionByDepotRequest
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    
    public required Guid DepotId { get; set; }
}