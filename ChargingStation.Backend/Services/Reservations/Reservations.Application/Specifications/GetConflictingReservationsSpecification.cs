using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace Reservations.Application.Specifications;

public class GetConflictingReservationsSpecification : Specification<Reservation>
{
    public GetConflictingReservationsSpecification(DateTime startDateTime, DateTime expiryDateTime, TimeSpan gapBetweenReservations, Guid chargePointId, Guid connectorId)
    {
        AddFilter(r =>
            (r.StartDateTime >= startDateTime && startDateTime <= r.ExpiryDateTime + gapBetweenReservations) ||
            (r.ExpiryDateTime <= expiryDateTime + gapBetweenReservations && expiryDateTime >= r.StartDateTime));
        AddFilter(r => r.ConnectorId == connectorId);
        AddFilter(r => r.ChargePointId == chargePointId);
        AddFilter(r => !r.IsCancelled);
    }
}