namespace ChargingStation.SignalR.Models;

public class TransactionMessage : BaseMassage
{
    public Guid ConnectorId { get; set; }
    public int TransactionId { get; set; }
}