namespace Reservations.Application.Models.Requests;

public class UpdateReservationRequest
{
    public Guid Id { get; set; }
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime ExpiryDateTime { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}