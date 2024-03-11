using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Responses.Enums;

[GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v9.0.0.0)")]
public enum ClearChargingProfileResponseStatus
{
    [EnumMember(Value = @"Accepted")]
    Accepted = 0,

    [EnumMember(Value = @"Unknown")]
    Unknown = 1
}
