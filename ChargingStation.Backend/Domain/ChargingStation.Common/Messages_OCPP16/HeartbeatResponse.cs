namespace ChargingStation.Common.Messages_OCPP16;
#pragma warning disable // Disable all warnings

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public class HeartbeatResponse
{
    [Newtonsoft.Json.JsonProperty("currentTime", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    public DateTimeOffset CurrentTime { get; set; }
}