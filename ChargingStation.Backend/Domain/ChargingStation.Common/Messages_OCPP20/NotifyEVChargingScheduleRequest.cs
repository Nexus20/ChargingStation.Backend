namespace ChargingStation.Common.Messages_OCPP20;
#pragma warning disable // Disable all warnings

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class NotifyEVChargingScheduleRequest
{
    [Newtonsoft.Json.JsonProperty("customData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CustomDataType CustomData { get; set; }

    /// <summary>Periods contained in the charging profile are relative to this point in time.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("timeBase", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    public System.DateTimeOffset TimeBase { get; set; }

    [Newtonsoft.Json.JsonProperty("chargingSchedule", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required]
    public ChargingScheduleType ChargingSchedule { get; set; } = new ChargingScheduleType();

    /// <summary>The charging schedule contained in this notification applies to an EVSE. EvseId must be &amp;gt; 0.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("evseId", Required = Newtonsoft.Json.Required.Always)]
    public int EvseId { get; set; }
}