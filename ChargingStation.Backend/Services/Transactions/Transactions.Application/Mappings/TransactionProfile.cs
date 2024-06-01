using AutoMapper;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.Transactions.Responses;
using ChargingStation.Domain.Entities;

namespace Transactions.Application.Mappings;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<OcppTransaction, TransactionResponse>()
            .ForMember(d => d.ReservationId, o => o.MapFrom(s => s.Reservation != null ? s.Reservation.Id : (Guid?)null));
        CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
        CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
            .As(typeof(PagedCollection<>));
    }
}