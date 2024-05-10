using ChargingStation.Common.Models.Abstract;

namespace ChargingStation.ChargingProfiles.Models.Responses;

public class ChargingSchedulePeriodResponse : BaseResponse
{
    public int StartPeriod { get; set; }
    public double Limit { get; set; }
    public int NumberPhases { get; set; }
}