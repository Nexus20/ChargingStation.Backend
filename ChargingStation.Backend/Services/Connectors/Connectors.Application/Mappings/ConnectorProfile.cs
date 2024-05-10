using AutoMapper;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.Domain.Entities;

namespace Connectors.Application.Mappings;

public class ConnectorProfile : Profile
{
    public ConnectorProfile()
    {
        CreateMap<GetOrCreateConnectorRequest, Connector>();
        CreateMap<Connector, ConnectorResponse>()
            .ForMember(d => d.CurrentStatus, o => o.Ignore());
    }
}