using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace Reservations.Application.Specifications;

public class GetReservationByRequestIdSpecification : Specification<Reservation>
{
    public GetReservationByRequestIdSpecification(string reservationRequestId)
    {
        AddFilter(r => r.ReservationRequestId == reservationRequestId);
    }
}