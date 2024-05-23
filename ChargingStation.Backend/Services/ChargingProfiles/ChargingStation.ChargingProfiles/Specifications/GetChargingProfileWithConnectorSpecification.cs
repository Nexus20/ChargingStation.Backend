using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetChargingProfileWithConnectorSpecification : Specification<ChargingProfile>
{
    public GetChargingProfileWithConnectorSpecification(Guid id)
    {
        AddFilter(x => x.Id == id);
        AddInclude(nameof(ChargingProfile.ConnectorChargingProfiles));
    }
}