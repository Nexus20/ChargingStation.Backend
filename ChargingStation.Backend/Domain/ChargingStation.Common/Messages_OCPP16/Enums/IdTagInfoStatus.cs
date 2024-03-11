using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Enums;

public enum IdTagInfoStatus
{
    [EnumMember(Value = @"Accepted")]
    Accepted = 0,

    [EnumMember(Value = @"Blocked")]
    Blocked = 1,

    [EnumMember(Value = @"Expired")]
    Expired = 2,

    [EnumMember(Value = @"Invalid")]
    Invalid = 3,

    [EnumMember(Value = @"ConcurrentTx")]
    ConcurrentTx = 4
}
