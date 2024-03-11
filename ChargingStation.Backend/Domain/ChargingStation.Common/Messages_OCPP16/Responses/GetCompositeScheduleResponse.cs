using System.ComponentModel.DataAnnotations;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChargingStation.Common.Messages_OCPP16.Responses;

public record GetCompositeScheduleResponse(
    [property: JsonProperty("status", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: JsonConverter(typeof(StringEnumConverter))]
    GetCompositeScheduleResponseStatus Status)
{
    [JsonProperty("connectorId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? ConnectorId { get; init; }

    [JsonProperty("scheduleStart", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? ScheduleStart { get; init; }

    [JsonProperty("chargingSchedule", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public ChargingSchedule? ChargingSchedule { get; init; }
}
