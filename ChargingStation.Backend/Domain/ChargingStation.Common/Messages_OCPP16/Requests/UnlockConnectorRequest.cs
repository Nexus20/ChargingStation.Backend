using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record UnlockConnectorRequest(
    [property: JsonProperty("connectorId", Required = Required.Always)]
    int ConnectorId);
