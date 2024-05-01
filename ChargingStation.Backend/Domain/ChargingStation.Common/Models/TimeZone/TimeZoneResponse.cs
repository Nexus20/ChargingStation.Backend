using ChargingStation.Common.Models.Abstract;

namespace ChargingStation.Common.Models.TimeZone;

public class TimeZoneResponse : BaseResponse
{
    public required string DisplayName { get; set; }
    public required TimeSpan BaseUtcOffset { get; set; }
    public required string IanaId { get; set; }
    public required string WindowsId { get; set; }
}

