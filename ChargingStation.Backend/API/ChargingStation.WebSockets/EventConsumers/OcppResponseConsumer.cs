using System.Text;
using ChargingStation.Common.Models;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppConnectionHandlers;
using MassTransit;

namespace ChargingStation.WebSockets.EventConsumers;

public class OcppResponseConsumer : IConsumer<ResponseIntegrationOcppMessage>
{
    private readonly ILogger<OcppResponseConsumer> _logger;
    private readonly IOcppWebSocketConnectionHandler _ocppWebSocketConnectionHandler;

    public OcppResponseConsumer(ILogger<OcppResponseConsumer> logger, IOcppWebSocketConnectionHandler ocppWebSocketConnectionHandler)
    {
        _logger = logger;
        _ocppWebSocketConnectionHandler = ocppWebSocketConnectionHandler;
    }

    public async Task Consume(ConsumeContext<ResponseIntegrationOcppMessage> context)
    {
        _logger.LogInformation("Received OCPP response message: {OcppMessageId}", context.Message.OcppMessageId);
        
        var payload = Encoding.UTF8.GetString(Convert.FromBase64String(context.Message.Payload[3..^3]));
        
        var messageOut = new OcppMessage
        {
            MessageType = "3",
            UniqueId = context.Message.OcppMessageId,
            JsonPayload = payload,
        };
        
        await _ocppWebSocketConnectionHandler.SendResponseAsync(context.Message.ChargePointId, messageOut);
    }
}