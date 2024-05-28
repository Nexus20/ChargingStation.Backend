using ChargingStation.Common.Messages_OCPP16.Responses.Enums;

namespace ChargingStation.InternalCommunication.SignalRModels;

public class ChangeAvailabilityMessage : BaseMessage
{
    public int? ConnectorId { get; set; }
    public ChangeAvailabilityResponseStatus Status { get; set; }
}