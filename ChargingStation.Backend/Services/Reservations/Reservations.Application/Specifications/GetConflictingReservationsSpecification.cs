using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace Reservations.Application.Specifications;

public class GetConflictingReservationsSpecification : Specification<Reservation>
{
    public GetConflictingReservationsSpecification(DateTime startDateTime, DateTime expiryDateTime, TimeSpan gapBetweenReservations, Guid chargePointId, Guid connectorId)
    {
        AddFilter(r => r.StartDateTime >= startDateTime || r.ExpiryDateTime <= expiryDateTime + gapBetweenReservations);
        AddFilter(r => r.ConnectorId == connectorId);
        AddFilter(r => r.ChargePointId == chargePointId);
        AddFilter(r => !r.IsCancelled);
    }
}