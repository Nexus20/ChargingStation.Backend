using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Reservations.Services.Reservations;
using MassTransit;

namespace ChargingStation.Reservations.EventConsumers;

public class ReserveNowResponseConsumer : IConsumer<IntegrationOcppMessage<ReserveNowResponse>>
{
    private readonly ILogger<ReserveNowResponseConsumer> _logger;
    private readonly IReservationService _reservationService;

    public ReserveNowResponseConsumer(ILogger<ReserveNowResponseConsumer> logger, IReservationService reservationService)
    {
        _logger = logger;
        _reservationService = reservationService;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<ReserveNowResponse>> context)
    {
        _logger.LogInformation("Processing reservation response message...");
        
        var reservationResponse = context.Message.Payload;
        var ocppMessageId = context.Message.OcppMessageId;

        await _reservationService.ProcessReservationResponseAsync(reservationResponse, ocppMessageId, context.CancellationToken);
    }
}