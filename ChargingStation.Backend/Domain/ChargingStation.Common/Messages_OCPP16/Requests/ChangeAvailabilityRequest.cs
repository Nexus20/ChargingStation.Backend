using System.ComponentModel.DataAnnotations;
using ChargingStation.Common.Messages_OCPP16.Requests.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record ChangeAvailabilityRequest(
    [property: JsonProperty("connectorId", Required = Required.Always)]
    int ConnectorId,
    [property: JsonProperty("type", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: JsonConverter(typeof(StringEnumConverter))]
    ChangeAvailabilityRequestType Type);
