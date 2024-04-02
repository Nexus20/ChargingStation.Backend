using AutoMapper;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.OcppTags.Responses;
using ChargingStation.Domain.Entities;
using ChargingStation.OcppTags.Models.Requests;

namespace ChargingStation.OcppTags.Mappings;

public class OcppTagProfile : Profile
{
    public OcppTagProfile()
    {
        CreateMap<CreateOcppTagRequest, OcppTag>();
        CreateMap<OcppTag, OcppTagResponse>();
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}