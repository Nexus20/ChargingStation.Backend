using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record SetChargingProfileRequest(
    [property: JsonProperty("connectorId", Required = Required.Always)]
    int ConnectorId,
    [property: JsonProperty("csChargingProfiles", Required = Required.Always)]
    [property: Required]
    CsChargingProfiles CsChargingProfiles);
