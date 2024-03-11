using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record UpdateFirmwareRequest(
    [property: JsonProperty("location", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    Uri Location,
    [property: JsonProperty("retrieveDate", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    DateTimeOffset RetrieveDate)
{
    [JsonProperty("retries", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? Retries { get; init; }

    [JsonProperty("retryInterval", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? RetryInterval { get; init; }
}
