using AutoMapper;
using ChargingStation.Aggregator.Models.Responses;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General;

namespace ChargingStation.Aggregator.Mappings;

public class DepotProfile : Profile
{
    public DepotProfile()
    {
        CreateMap<DepotResponse, DepotAggregatedResponse>();
        CreateMap<DepotResponse, DepotAggregatedDetailsResponse>();
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}