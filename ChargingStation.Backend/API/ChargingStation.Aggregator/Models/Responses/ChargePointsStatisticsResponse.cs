namespace ChargingStation.Aggregator.Models.Responses;

public class ChargePointsStatisticsResponse
{
    public int Online { get; set; }
    public int Offline { get; set; }
    public int HasErrors { get; set; }
}