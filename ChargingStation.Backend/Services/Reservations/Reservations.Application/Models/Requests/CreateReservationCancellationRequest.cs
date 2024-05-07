namespace Reservations.Application.Models.Requests;

public class CreateReservationCancellationRequest
{
    public Guid ReservationId { get; set; }
}