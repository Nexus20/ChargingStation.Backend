namespace ChargingStation.Reservations.Models.Requests;

public class CreateReservationCancellationRequest
{
    public Guid ReservationId { get; set; }
}