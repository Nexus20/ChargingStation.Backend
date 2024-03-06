namespace ChargingStation.Common.Messages_OCPP16;
#pragma warning disable // Disable all warnings

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public class StartTransactionResponse
{
    [Newtonsoft.Json.JsonProperty("idTagInfo", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required]
    public IdTagInfo IdTagInfo { get; set; } = new IdTagInfo();

    [Newtonsoft.Json.JsonProperty("transactionId", Required = Newtonsoft.Json.Required.Always)]
    public int TransactionId { get; set; }
}