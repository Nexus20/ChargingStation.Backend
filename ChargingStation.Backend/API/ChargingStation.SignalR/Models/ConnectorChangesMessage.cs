namespace ChargingStation.SignalR.Models
{
    public class ConnectorChangesMessage : BaseMassage
    {
        public Guid ConnectorId { get; set; }
        public string? Status { get; set; }
        public double? Value { get; set; }
        public Guid? TransactionId { get; set; }
    }
}
