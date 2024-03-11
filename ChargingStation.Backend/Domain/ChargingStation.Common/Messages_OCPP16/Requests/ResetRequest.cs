using System.ComponentModel.DataAnnotations;
using ChargingStation.Common.Messages_OCPP16.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record ResetRequest(
    [property: JsonProperty("type", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: JsonConverter(typeof(StringEnumConverter))]
    ResetRequestType Type);
