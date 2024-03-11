using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Responses;

public record GetLocalListVersionResponse(
    [property: JsonProperty("listVersion", Required = Required.Always)]
    int ListVersion);
