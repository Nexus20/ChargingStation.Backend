﻿using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using Reservations.Application.Models.Requests;
using Reservations.Application.Models.Responses;

namespace Reservations.Application.Services.Reservations;

public interface IReservationService : IBaseReservationService
{
    Task CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);
    Task UpdateReservationAsync(UpdateReservationRequest request, CancellationToken cancellationToken = default);
    Task ProcessReservationResponseAsync(ReserveNowResponse reservationResponse, string ocppMessageId, CancellationToken cancellationToken = default);
    Task<ReservationCancellationCreationResultStatus> CreateReservationCancellation(CreateReservationCancellationRequest request, CancellationToken cancellationToken = default);
    Task ProcessReservationCancellationResponseAsync(CancelReservationResponse cancelReservationResponse, string ocppMessageId, CancellationToken cancellationToken = default);
    Task<ReservationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IPagedCollection<ReservationResponse>> GetAsync(GetReservationsRequest request, CancellationToken cancellationToken = default);
}