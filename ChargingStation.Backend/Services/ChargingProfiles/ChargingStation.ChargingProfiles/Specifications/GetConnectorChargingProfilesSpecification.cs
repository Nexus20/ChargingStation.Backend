using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetConnectorChargingProfilesSpecification : Specification<ConnectorChargingProfile>
{
    public GetConnectorChargingProfilesSpecification(Guid connectorId)
    {
        AddFilter(x => x.ConnectorId == connectorId);
    }
}