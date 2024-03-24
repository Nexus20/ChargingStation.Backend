using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models;
using ChargingStation.Reservations.Models.Requests;
using ChargingStation.Reservations.Models.Responses;

namespace ChargingStation.Reservations.Services.Reservations;

public interface IReservationService
{
    Task CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);
    Task ProcessReservationResponseAsync(ReserveNowResponse reservationResponse, string ocppMessageId, CancellationToken cancellationToken = default);
    Task CreateReservationCancellation(CreateReservationCancellationRequest request, CancellationToken cancellationToken = default);
    Task ProcessReservationCancellationResponseAsync(CancelReservationResponse cancelReservationResponse, string ocppMessageId, CancellationToken cancellationToken = default);
    Task<ReservationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IPagedCollection<ReservationResponse>> GetAsync(GetReservationsRequest request, CancellationToken cancellationToken = default);
}