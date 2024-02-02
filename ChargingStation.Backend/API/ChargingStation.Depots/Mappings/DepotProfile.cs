using AutoMapper;
using ChargingStation.Common.Models;
using ChargingStation.Depots.Models.Responses;
using ChargingStation.Domain.Entities;

namespace ChargingStation.Depots.Mappings;

public class DepotProfile : Profile
{
    public DepotProfile()
    {
        CreateMap<Depot, DepotResponse>();
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}