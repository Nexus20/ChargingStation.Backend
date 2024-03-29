using ChargingStation.Common.Messages_OCPP16.Requests.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record ClearChargingProfileRequest
{
    [JsonProperty("id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; init; }

    [JsonProperty("connectorId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? ConnectorId { get; init; }

    [JsonProperty("chargingProfilePurpose", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(StringEnumConverter))]
    public ClearChargingProfileRequestChargingProfilePurpose? ChargingProfilePurpose { get; init; }

    [JsonProperty("stackLevel", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? StackLevel { get; init; }
}
