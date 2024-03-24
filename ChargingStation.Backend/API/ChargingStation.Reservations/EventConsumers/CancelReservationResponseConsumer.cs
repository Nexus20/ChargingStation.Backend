using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models;
using ChargingStation.Common.Models.General;
using ChargingStation.Reservations.Services;
using ChargingStation.Reservations.Services.Reservations;
using MassTransit;

namespace ChargingStation.Reservations.EventConsumers;

public class CancelReservationResponseConsumer : IConsumer<IntegrationOcppMessage<CancelReservationResponse>>
{
    private readonly ILogger<CancelReservationResponseConsumer> _logger;
    private readonly IReservationService _reservationService;

    public CancelReservationResponseConsumer(ILogger<CancelReservationResponseConsumer> logger, IReservationService reservationService)
    {
        _logger = logger;
        _reservationService = reservationService;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<CancelReservationResponse>> context)
    {
        _logger.LogInformation("Processing reservation cancellation response message...");
        
        var cancellationResponse = context.Message.Payload;
        var ocppMessageId = context.Message.OcppMessageId;

        await _reservationService.ProcessReservationCancellationResponseAsync(cancellationResponse, ocppMessageId, context.CancellationToken);
    }
}