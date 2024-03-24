using AutoMapper;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.Domain.Entities;

namespace ChargingStation.Connectors.Mappings;

public class ConnectorProfile : Profile
{
    public ConnectorProfile()
    {
        CreateMap<GetOrCreateConnectorRequest, Connector>();
        CreateMap<Connector, ConnectorResponse>();
    }
}