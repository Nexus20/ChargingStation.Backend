using AutoMapper;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Depots.Models.Requests;
using ChargingStation.Domain.Entities;

namespace ChargingStation.Depots.Mappings;

public class DepotProfile : Profile
{
    public DepotProfile()
    {
        CreateMap<CreateDepotRequest, Depot>();
        CreateMap<UpdateDepotRequest, Depot>();
        CreateMap<Depot, DepotResponse>();
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}