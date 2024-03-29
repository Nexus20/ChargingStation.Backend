using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record MeterValuesRequest(
    [property: JsonProperty("connectorId", Required = Required.Always)]
    int ConnectorId,
    [property: JsonProperty("meterValue", Required = Required.Always)]
    [property: Required]
    ICollection<MeterValue> MeterValue)
{
    [JsonProperty("transactionId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? TransactionId { get; init; }
}
