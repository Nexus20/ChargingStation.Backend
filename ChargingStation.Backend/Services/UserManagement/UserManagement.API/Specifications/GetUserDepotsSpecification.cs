using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace UserManagement.API.Specifications;

public class GetUserDepotsSpecification : Specification<ApplicationUserDepot>
{
    public GetUserDepotsSpecification(Guid userId)
    {
        AddFilter(d => d.ApplicationUserId == userId);
    }
}