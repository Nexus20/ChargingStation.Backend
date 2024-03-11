using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Responses;

public record StartTransactionResponse
{
    [JsonProperty("idTagInfo", Required = Required.Always)]
    [Required]
    public IdTagInfo IdTagInfo { get; set; } = new IdTagInfo();

    [JsonProperty("transactionId", Required = Required.Always)]
    public int TransactionId { get; set; }
}
