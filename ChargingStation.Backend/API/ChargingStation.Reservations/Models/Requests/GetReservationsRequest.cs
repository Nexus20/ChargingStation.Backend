namespace ChargingStation.Reservations.Models.Requests;

public class GetReservationsRequest
{
    public DateTime? StartDateTime { get; set; }
    public DateTime? ExpiryDateTime { get; set; }
    public Guid? ChargePointId { get; set; }
    public Guid? ConnectorId { get; set; }
    public Guid? TagId { get; set; }
    public string? Status { get; set; }
    public bool IsCancelled { get; set; }
}