using ChargingStation.Common.Models.General.Requests;

namespace ChargingStation.Depots.Models.Requests;

public class GetTimeZoneRequest : BaseCollectionRequest
{
    public string? DisplayName { get; set; }
    public TimeSpan? BaseUtcOffset { get; set; }
    public string? IanaId { get; set; }
    public string? WindowsId { get; set; }
}