using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace UserManagement.API.Specifications;

public class GetApplicationUserDepotSpecification : Specification<ApplicationUserDepot>
{
    public GetApplicationUserDepotSpecification(Guid depotId, Guid userId)
    {
        AddFilter(d => d.DepotId == depotId && d.ApplicationUserId == userId);
    }
}