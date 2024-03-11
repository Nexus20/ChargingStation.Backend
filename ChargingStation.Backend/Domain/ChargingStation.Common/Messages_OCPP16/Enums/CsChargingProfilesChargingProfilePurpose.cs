using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Enums;

public enum CsChargingProfilesChargingProfilePurpose
{
    [EnumMember(Value = @"ChargePointMaxProfile")]
    ChargePointMaxProfile = 0,

    [EnumMember(Value = @"TxDefaultProfile")]
    TxDefaultProfile = 1,

    [EnumMember(Value = @"TxProfile")]
    TxProfile = 2
}
