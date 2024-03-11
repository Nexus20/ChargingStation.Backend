using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record RemoteStopTransactionRequest(
    [property: JsonProperty("transactionId", Required = Required.Always)]
    int TransactionId);
