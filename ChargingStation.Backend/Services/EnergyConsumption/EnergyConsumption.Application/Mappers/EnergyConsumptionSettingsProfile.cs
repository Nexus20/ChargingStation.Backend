using AutoMapper;
using ChargingStation.ChargingProfiles.Models.Requests.ConsumptionSettings;
using ChargingStation.Common.Models.DepotEnergyConsumption;
using ChargingStation.Common.Models.DepotEnergyConsumption.Dtos;
using ChargingStation.Domain.Entities;

namespace ChargingStation.ChargingProfiles.Mappers;

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