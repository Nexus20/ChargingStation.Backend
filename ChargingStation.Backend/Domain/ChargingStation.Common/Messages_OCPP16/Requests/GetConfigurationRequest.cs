using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record GetConfigurationRequest
{
    [JsonProperty("key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public ICollection<string>? Key { get; init; }
}
