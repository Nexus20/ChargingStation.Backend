using AutoMapper;
using ChargingStation.Connectors.Models.Requests;
using ChargingStation.Domain.Entities;

namespace ChargingStation.Connectors.Mappings;

public class ConnectorStatusProfile : Profile
{
    public ConnectorStatusProfile()
    {
        CreateMap<UpdateConnectorStatusRequest, ConnectorStatus>();
    }
}