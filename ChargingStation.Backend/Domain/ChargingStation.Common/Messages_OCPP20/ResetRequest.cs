namespace ChargingStation.Common.Messages_OCPP20;
#pragma warning disable // Disable all warnings

/// <summary>This contains the type of reset that the Charging Station or EVSE should perform.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public enum ResetEnumType
{
    [System.Runtime.Serialization.EnumMember(Value = @"Immediate")]
    Immediate = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"OnIdle")]
    OnIdle = 1
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class ResetRequest
{
    [Newtonsoft.Json.JsonProperty("customData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CustomDataType CustomData { get; set; }

    [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public ResetEnumType Type { get; set; }

    /// <summary>This contains the ID of a specific EVSE that needs to be reset, instead of the entire Charging Station.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("evseId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int EvseId { get; set; }
}