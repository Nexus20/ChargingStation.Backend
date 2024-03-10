namespace ChargingStation.Common.Messages_OCPP16;
#pragma warning disable // Disable all warnings

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public class ResetResponse
{
    [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public ResetResponseStatus Status { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public enum ResetResponseStatus
{
    [System.Runtime.Serialization.EnumMember(Value = @"Accepted")]
    Accepted = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"Rejected")]
    Rejected = 1,
}