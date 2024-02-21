namespace ChargingStation.Common.Messages_OCPP20;
#pragma warning disable // Disable all warnings

/// <summary>This indicates whether the Charging Station has unlocked the connector.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public enum UnlockStatusEnumType
{
    [System.Runtime.Serialization.EnumMember(Value = @"Unlocked")]
    Unlocked = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"UnlockFailed")]
    UnlockFailed = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"OngoingAuthorizedTransaction")]
    OngoingAuthorizedTransaction = 2,

    [System.Runtime.Serialization.EnumMember(Value = @"UnknownConnector")]
    UnknownConnector = 3
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class UnlockConnectorResponse
{
    [Newtonsoft.Json.JsonProperty("customData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CustomDataType CustomData { get; set; }

    [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public UnlockStatusEnumType Status { get; set; }

    [Newtonsoft.Json.JsonProperty("statusInfo", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public StatusInfoType StatusInfo { get; set; }
}