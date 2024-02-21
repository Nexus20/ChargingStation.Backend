namespace ChargingStation.Common.Messages_OCPP20;
#pragma warning disable // Disable all warnings

/// <summary>Source of the charging limit.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public enum ChargingLimitSourceEnumType
{
    [System.Runtime.Serialization.EnumMember(Value = @"EMS")]
    EMS = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"Other")]
    Other = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"SO")]
    SO = 2,

    [System.Runtime.Serialization.EnumMember(Value = @"CSO")]
    CSO = 3,

}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class ClearedChargingLimitRequest
{
    [Newtonsoft.Json.JsonProperty("customData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CustomDataType CustomData { get; set; }

    [Newtonsoft.Json.JsonProperty("chargingLimitSource", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public ChargingLimitSourceEnumType ChargingLimitSource { get; set; }

    /// <summary>EVSE Identifier.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("evseId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int EvseId { get; set; }
}