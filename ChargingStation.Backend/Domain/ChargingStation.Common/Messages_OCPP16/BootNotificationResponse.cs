namespace ChargingStation.Common.Messages_OCPP16;
#pragma warning disable // Disable all warnings

public interface IOcppResponse
{
}

public interface IOcpp16Response : IOcppResponse
{
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class BootNotificationResponse : IOcpp16Response
{
    [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public BootNotificationResponseStatus Status { get; set; }

    [Newtonsoft.Json.JsonProperty("currentTime", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    public System.DateTimeOffset CurrentTime { get; set; }

    /// <summary>
    /// Heartbeat interval in seconds
    /// </summary>
    [Newtonsoft.Json.JsonProperty("interval", Required = Newtonsoft.Json.Required.Always)]
    public int Interval { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public enum BootNotificationResponseStatus
{
    [System.Runtime.Serialization.EnumMember(Value = @"Accepted")]
    Accepted = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"Pending")]
    Pending = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"Rejected")]
    Rejected = 2,
}