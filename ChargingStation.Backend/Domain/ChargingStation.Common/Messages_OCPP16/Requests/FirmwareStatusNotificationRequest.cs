using System.ComponentModel.DataAnnotations;
using ChargingStation.Common.Messages_OCPP16.Requests.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record FirmwareStatusNotificationRequest
{
    [JsonProperty("status", Required = Required.Always)]
    [Required(AllowEmptyStrings = true)]
    [JsonConverter(typeof(StringEnumConverter))]
    public FirmwareStatusNotificationRequestStatus Status { get; init; }
}
