namespace Transactions.Application.Models.EnergyConsumption.Requests;

public class GetDepotEnergyConsumptionStatisticsRequest
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public required TimeSpan AggregationInterval { get; set; }
    public required Guid DepotId { get; set; }
}