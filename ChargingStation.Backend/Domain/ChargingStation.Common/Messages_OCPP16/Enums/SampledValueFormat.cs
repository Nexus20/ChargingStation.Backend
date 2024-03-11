using System.Runtime.Serialization;

namespace ChargingStation.Common.Messages_OCPP16.Enums;

public enum SampledValueFormat
{
    [EnumMember(Value = @"Raw")]
    Raw = 0,

    [EnumMember(Value = @"SignedData")]
    SignedData = 1
}
