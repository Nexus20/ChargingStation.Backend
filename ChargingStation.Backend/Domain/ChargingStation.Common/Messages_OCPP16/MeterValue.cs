using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16;

public record MeterValue(
    [property: JsonProperty("timestamp", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    DateTimeOffset Timestamp,
    [property: JsonProperty("sampledValue", Required = Required.Always)]
    [property: Required]
    ICollection<SampledValue> SampledValue);
