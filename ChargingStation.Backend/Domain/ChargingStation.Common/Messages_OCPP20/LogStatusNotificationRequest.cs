namespace ChargingStation.Common.Messages_OCPP20;
#pragma warning disable // Disable all warnings

/// <summary>This contains the status of the log upload.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public enum UploadLogStatusEnumType
{
    [System.Runtime.Serialization.EnumMember(Value = @"BadMessage")]
    BadMessage = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"Idle")]
    Idle = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"NotSupportedOperation")]
    NotSupportedOperation = 2,

    [System.Runtime.Serialization.EnumMember(Value = @"PermissionDenied")]
    PermissionDenied = 3,

    [System.Runtime.Serialization.EnumMember(Value = @"Uploaded")]
    Uploaded = 4,

    [System.Runtime.Serialization.EnumMember(Value = @"UploadFailure")]
    UploadFailure = 5,

    [System.Runtime.Serialization.EnumMember(Value = @"Uploading")]
    Uploading = 6,

    [System.Runtime.Serialization.EnumMember(Value = @"AcceptedCanceled")]
    AcceptedCanceled = 7,

}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class LogStatusNotificationRequest
{
    [Newtonsoft.Json.JsonProperty("customData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CustomDataType CustomData { get; set; }

    [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public UploadLogStatusEnumType Status { get; set; }

    /// <summary>The request id that was provided in GetLogRequest that started this log upload. This field is mandatory,
    /// unless the message was triggered by a TriggerMessageRequest AND there is no log upload ongoing.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("requestId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int RequestId { get; set; }


}