using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Responses;

public record StopTransactionResponse
{
    [JsonProperty("idTagInfo", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public IdTagInfo? IdTagInfo { get; set; }
}
