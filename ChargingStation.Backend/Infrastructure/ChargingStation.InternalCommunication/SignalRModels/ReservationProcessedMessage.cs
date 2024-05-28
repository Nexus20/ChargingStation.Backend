using ChargingStation.Common.Messages_OCPP16.Responses.Enums;

namespace ChargingStation.InternalCommunication.SignalRModels;

public class ReservationProcessedMessage : BaseMessage
{
    public Guid ReservationId { get; set; }
    public Guid? ConnectorId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public ReserveNowResponseStatus Status { get; set; }
}