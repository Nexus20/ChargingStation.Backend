using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace Depots.Application.Specifications;

public class GetDepotSpecification : Specification<Depot>
{
    public GetDepotSpecification(Guid id)
    {
        AddFilter(d => d.Id == id);

        AddInclude(nameof(Depot.TimeZone));
    }
}