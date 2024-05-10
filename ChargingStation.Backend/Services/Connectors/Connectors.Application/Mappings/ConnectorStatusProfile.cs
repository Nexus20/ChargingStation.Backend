using AutoMapper;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.Domain.Entities;

namespace Connectors.Application.Mappings;

public class ConnectorStatusProfile : Profile
{
    public ConnectorStatusProfile()
    {
        CreateMap<UpdateConnectorStatusRequest, ConnectorStatus>();
        CreateMap<ConnectorStatus, ConnectorStatusResponse>();
    }
}