using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Enums;

public enum CsChargingProfilesChargingProfileKind
{
    [EnumMember(Value = @"Absolute")]
    Absolute = 0,

    [EnumMember(Value = @"Recurring")]
    Recurring = 1,

    [EnumMember(Value = @"Relative")]
    Relative = 2
}
