using AutoMapper;
using ChargePoints.Application.Models.Requests;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;

namespace ChargePoints.Application.Mappings;

public class ChargePointProfile : Profile
{
    public ChargePointProfile()
    {
        CreateMap<ChargePoint, ChargePointResponse>();
        CreateMap<CreateChargePointRequest, ChargePoint>();
        CreateMap<UpdateChargePointRequest, ChargePoint>();
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}