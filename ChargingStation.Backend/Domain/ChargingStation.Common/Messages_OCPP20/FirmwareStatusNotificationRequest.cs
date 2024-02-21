namespace ChargingStation.Common.Messages_OCPP20;
#pragma warning disable // Disable all warnings

/// <summary>This contains the progress status of the firmware installation.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public enum FirmwareStatusEnumType
{
    [System.Runtime.Serialization.EnumMember(Value = @"Downloaded")]
    Downloaded = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"DownloadFailed")]
    DownloadFailed = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"Downloading")]
    Downloading = 2,

    [System.Runtime.Serialization.EnumMember(Value = @"DownloadScheduled")]
    DownloadScheduled = 3,

    [System.Runtime.Serialization.EnumMember(Value = @"DownloadPaused")]
    DownloadPaused = 4,

    [System.Runtime.Serialization.EnumMember(Value = @"Idle")]
    Idle = 5,

    [System.Runtime.Serialization.EnumMember(Value = @"InstallationFailed")]
    InstallationFailed = 6,

    [System.Runtime.Serialization.EnumMember(Value = @"Installing")]
    Installing = 7,

    [System.Runtime.Serialization.EnumMember(Value = @"Installed")]
    Installed = 8,

    [System.Runtime.Serialization.EnumMember(Value = @"InstallRebooting")]
    InstallRebooting = 9,

    [System.Runtime.Serialization.EnumMember(Value = @"InstallScheduled")]
    InstallScheduled = 10,

    [System.Runtime.Serialization.EnumMember(Value = @"InstallVerificationFailed")]
    InstallVerificationFailed = 11,

    [System.Runtime.Serialization.EnumMember(Value = @"InvalidSignature")]
    InvalidSignature = 12,

    [System.Runtime.Serialization.EnumMember(Value = @"SignatureVerified")]
    SignatureVerified = 13,

}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class FirmwareStatusNotificationRequest
{
    [Newtonsoft.Json.JsonProperty("customData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CustomDataType CustomData { get; set; }

    [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public FirmwareStatusEnumType Status { get; set; }

    /// <summary>The request id that was provided in the
    /// UpdateFirmwareRequest that started this firmware update.
    /// This field is mandatory, unless the message was triggered by a TriggerMessageRequest AND there is no firmware update ongoing.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("requestId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int RequestId { get; set; }
}