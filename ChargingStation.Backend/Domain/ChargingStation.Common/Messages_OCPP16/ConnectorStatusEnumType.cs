namespace ChargingStation.Common.Messages_OCPP16;

#pragma warning disable
public enum ConnectorStatusEnumType
{
    [System.Runtime.Serialization.EnumMember(Value = "")]
    Undefined = 0,

    [System.Runtime.Serialization.EnumMember(Value = "Available")]
    Available = 1,

    [System.Runtime.Serialization.EnumMember(Value = "Occupied")]
    Occupied = 2,

    [System.Runtime.Serialization.EnumMember(Value = "Unavailable")]
    Unavailable = 3,

    [System.Runtime.Serialization.EnumMember(Value = "Faulted")]
    Faulted = 4
}