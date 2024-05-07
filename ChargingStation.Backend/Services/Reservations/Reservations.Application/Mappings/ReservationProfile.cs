using AutoMapper;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using Reservations.Application.Models.Responses;

namespace Reservations.Application.Mappings;

public class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<Reservation, ReservationResponse>();
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}