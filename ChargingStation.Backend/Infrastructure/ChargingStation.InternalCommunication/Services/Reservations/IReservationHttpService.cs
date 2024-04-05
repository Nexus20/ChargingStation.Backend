using ChargingStation.Common.Models.Reservations.Requests;

namespace ChargingStation.InternalCommunication.Services.Reservations;

public interface IReservationHttpService
{
    Task UseReservationAsync(UseReservationRequest request, CancellationToken cancellationToken = default);
}