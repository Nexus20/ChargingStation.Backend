using System.ComponentModel.DataAnnotations;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChargingStation.Common.Messages_OCPP16.Responses;

public record ChangeConfigurationResponse(
    [property: JsonProperty("status", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: JsonConverter(typeof(StringEnumConverter))]
    ChangeConfigurationResponseStatus Status);
