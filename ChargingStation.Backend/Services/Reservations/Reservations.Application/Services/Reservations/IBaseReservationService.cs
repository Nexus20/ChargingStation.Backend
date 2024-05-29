using ChargingStation.Common.Models.Reservations.Requests;

namespace Reservations.Application.Services.Reservations;

public interface IBaseReservationService
{
    Task UseReservationAsync(UseReservationRequest request, CancellationToken cancellationToken = default);
}