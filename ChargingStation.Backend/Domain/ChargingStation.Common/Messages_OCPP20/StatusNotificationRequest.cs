namespace ChargingStation.Common.Messages_OCPP20;
#pragma warning disable // Disable all warnings

/// <summary>This contains the current status of the Connector.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public enum ConnectorStatusEnumType
{
    [System.Runtime.Serialization.EnumMember(Value = @"Available")]
    Available = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"Occupied")]
    Occupied = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"Reserved")]
    Reserved = 2,

    [System.Runtime.Serialization.EnumMember(Value = @"Unavailable")]
    Unavailable = 3,

    [System.Runtime.Serialization.EnumMember(Value = @"Faulted")]
    Faulted = 4,

}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class StatusNotificationRequest
{
    [Newtonsoft.Json.JsonProperty("customData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CustomDataType CustomData { get; set; }

    /// <summary>The time for which the status is reported. If absent time of receipt of the message will be assumed.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    public System.DateTimeOffset Timestamp { get; set; }

    [Newtonsoft.Json.JsonProperty("connectorStatus", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public ConnectorStatusEnumType ConnectorStatus { get; set; }

    /// <summary>The id of the EVSE to which the connector belongs for which the the status is reported.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("evseId", Required = Newtonsoft.Json.Required.Always)]
    public int EvseId { get; set; }

    /// <summary>The id of the connector within the EVSE for which the status is reported.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("connectorId", Required = Newtonsoft.Json.Required.Always)]
    public int ConnectorId { get; set; }
}