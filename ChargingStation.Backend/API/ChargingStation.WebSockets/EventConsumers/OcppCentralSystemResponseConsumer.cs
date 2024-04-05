using System.Text;
using ChargingStation.Common.Constants;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppConnectionHandlers;
using MassTransit;

namespace ChargingStation.WebSockets.EventConsumers;

public class OcppCentralSystemResponseConsumer : IConsumer<CentralSystemResponseIntegrationOcppMessage>
{
    private readonly ILogger<OcppCentralSystemResponseConsumer> _logger;
    private readonly IOcppWebSocketConnectionHandler _ocppWebSocketConnectionHandler;

    public OcppCentralSystemResponseConsumer(ILogger<OcppCentralSystemResponseConsumer> logger, IOcppWebSocketConnectionHandler ocppWebSocketConnectionHandler)
    {
        _logger = logger;
        _ocppWebSocketConnectionHandler = ocppWebSocketConnectionHandler;
    }

    public async Task Consume(ConsumeContext<CentralSystemResponseIntegrationOcppMessage> context)
    {
        _logger.LogInformation("Received OCPP central system response message: {OcppMessageId}", context.Message.OcppMessageId);
        
        var payload = Encoding.UTF8.GetString(Convert.FromBase64String(context.Message.Payload[3..^3]));
        
        var messageOut = new OcppMessage
        {
            MessageType = OcppMessageTypes.CallResult,
            UniqueId = context.Message.OcppMessageId,
            JsonPayload = payload,
        };
        
        await _ocppWebSocketConnectionHandler.SendResponseAsync(context.Message.ChargePointId, messageOut);
    }
}