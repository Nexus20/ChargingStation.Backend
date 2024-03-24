using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.Reservations.Specifications;

public class GetReservationByCancellationRequestIdSpecification : Specification<Reservation>
{
    public GetReservationByCancellationRequestIdSpecification(string cancellationRequestId)
    {
        AddFilter(r => r.CancellationRequestId == cancellationRequestId);
    }
}