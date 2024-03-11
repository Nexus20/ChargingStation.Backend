using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Enums;

public enum ChargingScheduleChargingRateUnit
{
    [EnumMember(Value = @"A")]
    A = 0,

    [EnumMember(Value = @"W")]
    W = 1
}
