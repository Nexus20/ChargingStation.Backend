using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record ReserveNowRequest(
    [property: JsonProperty("connectorId", Required = Required.Always)]
    int ConnectorId,
    [property: JsonProperty("expiryDate", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    DateTimeOffset ExpiryDate,
    [property: JsonProperty("idTag", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: StringLength(20)]
    string IdTag,
    [property: JsonProperty("reservationId", Required = Required.Always)]
    int ReservationId)
{
    [JsonProperty("parentIdTag", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    [StringLength(20)]
    public string? ParentIdTag { get; init; }
}
