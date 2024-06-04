using AutoMapper;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Identity;
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

        CreateMap<UpdateUserRequest, ApplicationUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UpdateUserRequest, InfrastructureUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FirstName + src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}