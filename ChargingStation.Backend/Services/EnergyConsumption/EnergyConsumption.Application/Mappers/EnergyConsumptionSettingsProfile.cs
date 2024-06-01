using AutoMapper;
using ChargingStation.Common.Models.DepotEnergyConsumption;
using ChargingStation.Common.Models.DepotEnergyConsumption.Dtos;
using ChargingStation.Domain.Entities;
using EnergyConsumption.Application.Models.Requests;

namespace EnergyConsumption.Application.Mappers;

public class EnergyConsumptionSettingsProfile : Profile
{
    public EnergyConsumptionSettingsProfile()
    {
        CreateMap<SetDepotEnergyConsumptionSettingsRequest, DepotEnergyConsumptionSettings>();
        CreateMap<EnergyConsumptionIntervalSettingsDto, EnergyConsumptionIntervalSettings>()
            .ReverseMap();
        
        CreateMap<ChargePointEnergyConsumptionSettingsDto, ChargePointEnergyConsumptionSettings>()
            .ReverseMap();
        
        CreateMap<DepotEnergyConsumptionSettings, DepotEnergyConsumptionSettingsResponse>()
            .ForMember(dest => dest.ChargePointsLimits, opt => opt.MapFrom(src => src.ChargePointsLimits))
            .ForMember(dest => dest.Intervals, opt => opt.MapFrom(src => src.Intervals));
    }
}