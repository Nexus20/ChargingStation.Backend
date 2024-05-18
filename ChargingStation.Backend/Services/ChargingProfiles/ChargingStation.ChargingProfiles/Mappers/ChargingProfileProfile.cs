using AutoMapper;
using ChargingStation.ChargingProfiles.Models.Requests;
using ChargingStation.ChargingProfiles.Models.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;

namespace ChargingStation.ChargingProfiles.Mappers;

public class ChargingProfileProfile : Profile
{
    public ChargingProfileProfile()
    {
        CreateMap<ChargingProfile, ChargingProfileResponse>();
        CreateMap<ChargingSchedulePeriod, ChargingSchedulePeriodResponse>();

        CreateMap<CreateChargingProfileRequest, ChargingProfile>();
        CreateMap<ChargingSchedulePeriodRequest, ChargingSchedulePeriod>();
        
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}