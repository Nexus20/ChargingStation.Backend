using AutoMapper;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.WebSockets.Models;

namespace ChargingStation.WebSockets.Mappings;

public class ChargePointProfile : Profile
{
    public ChargePointProfile()
    {
        CreateMap<ChargePointInfo, ActiveChargePointResponse>();
    }
}