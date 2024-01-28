using AutoMapper;
using ChargingStation.Depots.Models.Responses;
using ChargingStation.Domain.Entities;

namespace ChargingStation.Depots.Mappings;

public class DepotProfile : Profile
{
    public DepotProfile()
    {
        CreateMap<Depot, DepotResponse>();
    }
}