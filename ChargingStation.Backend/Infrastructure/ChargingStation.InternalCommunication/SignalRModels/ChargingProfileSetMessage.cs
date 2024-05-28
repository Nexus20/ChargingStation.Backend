using ChargingStation.Common.Messages_OCPP16.Responses.Enums;

namespace ChargingStation.InternalCommunication.SignalRModels;

public class ChargingProfileSetMessage : BaseMessage
{
    public int ConnectorId { get; set; }
    public SetChargingProfileResponseStatus Status { get; set; }
    public Guid ChargingProfileId { get; set; }
}