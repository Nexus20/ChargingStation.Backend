namespace ChargingStation.InternalCommunication.SignalRModels;

public class ConnectorChangesMessage : BaseMessage
{
    public Guid ConnectorId { get; set; }
    public int? SoC { get; set; }
    public string? Status { get; set; }
    public double? Energy { get; set; }
    public int? TransactionId { get; set; }
}