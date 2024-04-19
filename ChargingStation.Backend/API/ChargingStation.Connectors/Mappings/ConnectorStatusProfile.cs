using AutoMapper;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.Domain.Entities;

namespace ChargingStation.Connectors.Mappings;

public class ConnectorStatusProfile : Profile
{
    public ConnectorStatusProfile()
    {
        CreateMap<UpdateConnectorStatusRequest, ConnectorStatus>();
        CreateMap<ConnectorStatus, ConnectorStatusResponse>();
    }
}