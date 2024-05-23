using AutoMapper;
using ChargePointEmulator.Application.Models;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.Domain.Entities;

namespace ChargePointEmulator.Application.Mappers;

public class ChargePointProfile : Profile
{
    public ChargePointProfile()
    {
        CreateMap<ChargePoint, ChargePointResponse>();
        CreateMap<ChargePoint, ChargePointSimulatorResponse>();
    }
}