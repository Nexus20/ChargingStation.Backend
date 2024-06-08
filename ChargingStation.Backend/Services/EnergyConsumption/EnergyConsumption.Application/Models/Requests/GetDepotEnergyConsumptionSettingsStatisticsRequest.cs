namespace EnergyConsumption.Application.Models.Requests;

public class GetDepotEnergyConsumptionSettingsStatisticsRequest
{
    public Guid DepotId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}