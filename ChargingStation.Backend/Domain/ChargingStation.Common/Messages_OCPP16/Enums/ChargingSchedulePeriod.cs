using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Enums;

[method: JsonConstructor]
public record ChargingSchedulePeriod(
    [property: JsonProperty("limit", Required = Required.Always)]
    double Limit,
    [property: JsonProperty("startPeriod", Required = Required.Always)]
    int StartPeriod)
{
    [JsonProperty("numberPhases", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? NumberPhases { get; init; }
}
