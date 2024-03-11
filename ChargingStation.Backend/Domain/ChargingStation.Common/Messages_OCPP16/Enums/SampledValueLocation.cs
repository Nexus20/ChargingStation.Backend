using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Enums;

public enum SampledValueLocation
{
    [EnumMember(Value = @"Cable")]
    Cable = 0,

    [EnumMember(Value = @"EV")]
    EV = 1,

    [EnumMember(Value = @"Inlet")]
    Inlet = 2,

    [EnumMember(Value = @"Outlet")]
    Outlet = 3,

    [EnumMember(Value = @"Body")]
    Body = 4
}
