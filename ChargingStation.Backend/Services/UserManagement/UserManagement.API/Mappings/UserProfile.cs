using AutoMapper;
using ChargingStation.Domain.Entities;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;

namespace UserManagement.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, ApplicationUser>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<ApplicationUser, UserResponse>()
            .ForMember(d => d.DepotsIds, opt => opt.MapFrom(s => s.ApplicationUserDepots != null ? s.ApplicationUserDepots.Select(d => d.DepotId).ToList() : null));
    }
}