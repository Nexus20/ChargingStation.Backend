using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace Connectors.Application.Specifications;

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