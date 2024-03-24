namespace ChargingStation.Reservations.Models.Requests;

public class CreateReservationRequest
{
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
    public Guid OcppTagId { get; set; }
    public DateTime ExpiryDateTime { get; set; }
}