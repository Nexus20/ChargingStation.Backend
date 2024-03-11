using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Enums;

public enum ResetRequestType
{
    [EnumMember(Value = @"Hard")]
    Hard = 0,

    [EnumMember(Value = @"Soft")]
    Soft = 1
}
