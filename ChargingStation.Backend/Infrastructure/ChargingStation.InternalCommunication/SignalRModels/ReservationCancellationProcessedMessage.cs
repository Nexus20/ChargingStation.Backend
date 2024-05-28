using ChargingStation.Common.Messages_OCPP16.Responses.Enums;

namespace ChargingStation.InternalCommunication.SignalRModels;

public class ReservationCancellationProcessedMessage : BaseMessage
{
    public Guid ReservationId { get; set; }
    public Guid? ConnectorId { get; set; }
    public CancelReservationResponseStatus Status { get; set; }
}