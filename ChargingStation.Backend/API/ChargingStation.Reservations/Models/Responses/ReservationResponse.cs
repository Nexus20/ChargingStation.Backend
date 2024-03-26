namespace ChargingStation.Reservations.Models.Responses;

public class ReservationResponse
{
    public Guid Id { get; set; }
    public int ReservationId { get; set; }
    public Guid ChargePointId { get; set; }
    public Guid? ConnectorId { get; set; }
    public Guid? TransactionId { get; set; }
    public Guid TagId { get; set; }
    public string? Status { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime ExpiryDateTime { get; set; }
    public bool IsCancelled { get; set; }
}