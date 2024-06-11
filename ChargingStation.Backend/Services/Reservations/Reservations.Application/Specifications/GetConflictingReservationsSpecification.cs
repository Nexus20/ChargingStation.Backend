using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace Reservations.Application.Specifications;

public class GetConflictingReservationsSpecification : Specification<Reservation>
{
    public GetConflictingReservationsSpecification(Guid chargePointId, Guid connectorId, Guid? reservationToSkipFromComparison = null)
    {
        if(reservationToSkipFromComparison.HasValue)
            AddFilter(r => r.Id != reservationToSkipFromComparison);
        
        AddFilter(r => r.ConnectorId == connectorId);
        AddFilter(r => r.ChargePointId == chargePointId);
        AddFilter(r => !r.IsCancelled);
    }
}