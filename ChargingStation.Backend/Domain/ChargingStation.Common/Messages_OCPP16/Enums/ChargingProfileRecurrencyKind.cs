using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Enums;

public enum ChargingProfileRecurrencyKind
{
    [EnumMember(Value = @"Daily")]
    Daily = 0,

    [EnumMember(Value = @"Weekly")]
    Weekly = 1
}
