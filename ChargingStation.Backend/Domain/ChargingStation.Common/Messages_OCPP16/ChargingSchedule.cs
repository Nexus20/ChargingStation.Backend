using System.ComponentModel.DataAnnotations;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChargingStation.Common.Messages_OCPP16;

public record ChargingSchedule(
    [property: JsonProperty("chargingRateUnit", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: JsonConverter(typeof(StringEnumConverter))]
    ChargingScheduleChargingRateUnit ChargingRateUnit,
    [property: JsonProperty("chargingSchedulePeriod", Required = Required.Always)]
    [property: Required]
    ICollection<ChargingSchedulePeriod> ChargingSchedulePeriod)
{
    [JsonProperty("duration", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? Duration { get; init; }

    [JsonProperty("startSchedule", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? StartSchedule { get; init; }

    [JsonProperty("minChargingRate", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public double? MinChargingRate { get; init; }

    public void Deconstruct(out ICollection<ChargingSchedulePeriod> chargingSchedulePeriod)
    {
        chargingSchedulePeriod = ChargingSchedulePeriod;
    }
}
