using AutoMapper;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Domain.Entities;

namespace ChargingStation.Connectors.Mappings;

public class ConnectorStatusProfile : Profile
{
    public ConnectorStatusProfile()
    {
        CreateMap<UpdateConnectorStatusRequest, ConnectorStatus>();
    }
}