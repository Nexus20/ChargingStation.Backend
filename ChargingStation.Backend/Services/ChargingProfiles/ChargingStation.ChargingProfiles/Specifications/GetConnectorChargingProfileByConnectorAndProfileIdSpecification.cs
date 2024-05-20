using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetConnectorChargingProfileByConnectorAndProfileIdSpecification : Specification<ConnectorChargingProfile>
{
    public GetConnectorChargingProfileByConnectorAndProfileIdSpecification(Guid connectorId, Guid chargingProfileId)
    {
        AddFilter(x => x.ConnectorId == connectorId && x.ChargingProfileId == chargingProfileId);
    }
}