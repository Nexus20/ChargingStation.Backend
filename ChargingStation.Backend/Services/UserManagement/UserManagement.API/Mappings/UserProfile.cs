using AutoMapper;
using ChargingStation.Domain.Entities;
using UserManagement.API.Models.Requests;

namespace UserManagement.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
