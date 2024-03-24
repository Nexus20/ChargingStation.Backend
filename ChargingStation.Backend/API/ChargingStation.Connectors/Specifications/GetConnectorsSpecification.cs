using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.Connectors.Specifications;

public class GetConnectorsSpecification : Specification<Connector>
{
    public GetConnectorsSpecification(GetOrCreateConnectorRequest request)
    {
        AddFilter(x => x.ChargePointId == request.ChargePointId);
        AddFilter(x => x.ConnectorId == request.ConnectorId);
    }

    public GetConnectorsSpecification(UpdateConnectorStatusRequest request)
    {
        AddFilter(x => x.ChargePointId == request.ChargePointId);
        AddFilter(x => x.ConnectorId == request.ConnectorId);
    }
}