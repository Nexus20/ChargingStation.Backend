using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record ChangeConfigurationRequest(
    [property: JsonProperty("key", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: StringLength(50)]
    string Key,
    [property: JsonProperty("value", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: StringLength(500)]
    string Value);
