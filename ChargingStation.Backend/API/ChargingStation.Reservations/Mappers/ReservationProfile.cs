using AutoMapper;
using ChargingStation.Domain.Entities;
using ChargingStation.Reservations.Models.Responses;

namespace ChargingStation.Reservations.Mappers;

public class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<Reservation, ReservationResponse>();
    }
}