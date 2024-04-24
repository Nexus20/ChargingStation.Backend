using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargePoints.Specifications;

public class GetChargePointsByIdsSpecification : Specification<ChargePoint>
{
    public GetChargePointsByIdsSpecification(ICollection<Guid> chargePointsIds)
    {
        AddFilter(c => chargePointsIds.Contains(c.Id));
    }
}