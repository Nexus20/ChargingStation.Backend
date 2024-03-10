using AutoMapper;
using ChargingStation.Connectors.Models.Requests;
using ChargingStation.Connectors.Models.Responses;
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