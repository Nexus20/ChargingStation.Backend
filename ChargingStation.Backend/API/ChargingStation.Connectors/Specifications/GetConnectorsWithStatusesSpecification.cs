using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.Connectors.Specifications;

public class GetConnectorsWithStatusesSpecification : Specification<Connector>
{
    public GetConnectorsWithStatusesSpecification()
    {
        AddInclude(nameof(Connector.ConnectorStatuses));
    }
    
    public GetConnectorsWithStatusesSpecification(ICollection<Guid> chargePointsIds) : this()
    {
        AddFilter(x => chargePointsIds.Contains(x.ChargePointId));
    }
}