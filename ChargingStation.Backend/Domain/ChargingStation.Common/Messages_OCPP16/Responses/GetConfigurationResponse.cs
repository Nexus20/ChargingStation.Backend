using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Responses;

public record GetConfigurationResponse
{
    [JsonProperty("configurationKey", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public ICollection<ConfigurationKey>? ConfigurationKey { get; init; }

    [JsonProperty("unknownKey", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public ICollection<string>? UnknownKey { get; init; }
}
