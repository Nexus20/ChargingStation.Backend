using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models;
using ChargingStation.WebSockets.OcppConnectionHandlers;
using MassTransit;

namespace ChargingStation.WebSockets.EventConsumers;

public class ResetConsumer : IConsumer<IntegrationOcppMessage<ResetRequest>>
{
    private readonly ILogger<ResetConsumer> _logger;
    private readonly IOcppWebSocketConnectionHandler _ocppWebSocketConnectionHandler;

    public ResetConsumer(ILogger<ResetConsumer> logger, IOcppWebSocketConnectionHandler ocppWebSocketConnectionHandler)
    {
        _logger = logger;
        _ocppWebSocketConnectionHandler = ocppWebSocketConnectionHandler;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<ResetRequest>> context)
    {
        _logger.LogInformation("Received OCPP reset message: {OcppMessageId}", context.Message.OcppMessageId);
        
        var incomingRequest = context.Message.Payload;
        var chargePointId = context.Message.ChargePointId;
        
        await _ocppWebSocketConnectionHandler.SendResetAsync(chargePointId, incomingRequest);
        _logger.LogInformation("Sent OCPP reset message: {OcppMessageId}", context.Message.OcppMessageId);
    }
}