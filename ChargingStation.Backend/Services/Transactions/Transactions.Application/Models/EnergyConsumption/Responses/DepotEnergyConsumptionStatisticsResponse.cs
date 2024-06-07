namespace Transactions.Application.Models.EnergyConsumption.Responses;

public class DepotEnergyConsumptionStatisticsResponse
{
    public DateTime DateTime { get; set; }
    public decimal AggregatedValue { get; set; }
    public decimal CumulativeValue { get; set; }
}