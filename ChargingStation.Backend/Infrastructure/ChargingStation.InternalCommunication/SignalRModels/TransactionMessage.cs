namespace ChargingStation.InternalCommunication.SignalRModels;

public class TransactionMessage : BaseMessage
{
    public Guid ConnectorId { get; set; }
    public int TransactionId { get; set; }
}