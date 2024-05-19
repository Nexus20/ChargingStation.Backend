using AutoMapper;
using ChargingStation.Common.Models.General;

namespace ChargePointEmulator.Application.Mappers;

public class PagedCollectionProfile : Profile
{
    public PagedCollectionProfile()
    {
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}