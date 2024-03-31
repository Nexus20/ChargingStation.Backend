namespace ChargingStation.ChargingProfiles.Models.Requests;

public class ChargingSchedulePeriodRequest
{
    public int StartPeriod { get; set; }
    public double Limit { get; set; }
    public int NumberPhases { get; set; }
}