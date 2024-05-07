using AutoMapper;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.OcppTags.Responses;
using ChargingStation.Domain.Entities;
using OcppTags.Application.Models.Requests;

namespace OcppTags.Application.Mappings;

public class OcppTagProfile : Profile
{
    public OcppTagProfile()
    {
        CreateMap<CreateOcppTagRequest, OcppTag>();
        CreateMap<UpdateOcppTagRequest, OcppTag>();
        CreateMap<OcppTag, OcppTagResponse>();
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}