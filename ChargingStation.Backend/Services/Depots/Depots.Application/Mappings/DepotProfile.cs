using AutoMapper;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using Depots.Application.Models.Requests;

namespace Depots.Application.Mappings;

public class DepotProfile : Profile
{
    public DepotProfile()
    {
        CreateMap<CreateDepotRequest, Depot>();
        CreateMap<UpdateDepotRequest, Depot>();
        CreateMap<Depot, DepotResponse>()
            .ForMember(dest => dest.BaseUtcOffset, opt => opt.MapFrom(src => src.TimeZone.BaseUtcOffset))
            .ForMember(dest => dest.IanaId, opt => opt.MapFrom(src => src.TimeZone.IanaId));
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}