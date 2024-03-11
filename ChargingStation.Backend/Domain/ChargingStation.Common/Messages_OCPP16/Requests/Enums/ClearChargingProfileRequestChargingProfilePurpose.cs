using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Requests.Enums;

[GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v9.0.0.0)")]
public enum ClearChargingProfileRequestChargingProfilePurpose
{
    [EnumMember(Value = @"ChargePointMaxProfile")]
    ChargePointMaxProfile = 0,

    [EnumMember(Value = @"TxDefaultProfile")]
    TxDefaultProfile = 1,

    [EnumMember(Value = @"TxProfile")]
    TxProfile = 2
}
