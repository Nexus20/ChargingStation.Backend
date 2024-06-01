using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using ChargingStation.Common.Models.Reservations.Requests;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using Reservations.Application.Specifications;

namespace Reservations.Application.Services.Reservations;

public class BaseReservationService : IBaseReservationService
{
    protected readonly IRepository<Reservation> ReservationRepository;

    public BaseReservationService(IRepository<Reservation> reservationRepository)
    {
        ReservationRepository = reservationRepository;
    }

    public async Task UseReservationAsync(UseReservationRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetReservationsSpecification(request);
        
        var reservation = await ReservationRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (reservation is null)
        {
            throw new NotFoundException($"Reservation with id {request.ReservationId} for charge point with id {request.ChargePointId} not found");
        }
        
        if (reservation.Status != ReserveNowResponseStatus.Accepted.ToString())
        {
            throw new BadRequestException($"Reservation with id {request.ReservationId} for charge point with id {request.ChargePointId} is not accepted");
        }
        
        reservation.IsUsed = true;
        reservation.Status = "Used";
        
        ReservationRepository.Update(reservation);
        await ReservationRepository.SaveChangesAsync(cancellationToken);
    }
}