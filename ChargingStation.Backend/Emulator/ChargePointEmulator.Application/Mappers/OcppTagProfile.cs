using AutoMapper;
using ChargingStation.Common.Models.OcppTags.Responses;
using ChargingStation.Domain.Entities;

namespace ChargePointEmulator.Application.Mappers;

public class OcppTagProfile : Profile
{
    public OcppTagProfile()
    {
        CreateMap<OcppTag, OcppTagResponse>();
    }
}