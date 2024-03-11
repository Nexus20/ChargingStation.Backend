using System.ComponentModel.DataAnnotations;
using ChargingStation.Common.Messages_OCPP16.Requests.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record TriggerMessageRequest(
    [property: JsonProperty("requestedMessage", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: JsonConverter(typeof(StringEnumConverter))]
    TriggerMessageRequestRequestedMessage RequestedMessage)
{
    [JsonProperty("connectorId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? ConnectorId { get; init; }
}
