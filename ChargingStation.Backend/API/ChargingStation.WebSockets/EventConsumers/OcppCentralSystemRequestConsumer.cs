using System.Text;
using ChargingStation.Common.Constants;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppConnectionHandlers;
using MassTransit;

namespace ChargingStation.WebSockets.EventConsumers;

public class OcppCentralSystemRequestConsumer : IConsumer<CentralSystemRequestIntegrationOcppMessage>
{
    private readonly ILogger<OcppCentralSystemRequestConsumer> _logger;
    private readonly IOcppWebSocketConnectionHandler _ocppWebSocketConnectionHandler;

    public OcppCentralSystemRequestConsumer(ILogger<OcppCentralSystemRequestConsumer> logger, IOcppWebSocketConnectionHandler ocppWebSocketConnectionHandler)
    {
        _logger = logger;
        _ocppWebSocketConnectionHandler = ocppWebSocketConnectionHandler;
    }

    public async Task Consume(ConsumeContext<CentralSystemRequestIntegrationOcppMessage> context)
    {
        _logger.LogInformation("Received OCPP central system request message: {OcppMessageId}", context.Message.OcppMessageId);
        
        var payload = Encoding.UTF8.GetString(Convert.FromBase64String(context.Message.Payload[3..^3]));
        
        var messageOut = new OcppMessage
        {
            MessageType = OcppMessageTypes.Call,
            UniqueId = context.Message.OcppMessageId,
            Action = context.Message.Action,
            JsonPayload = payload,
        };
        
        await _ocppWebSocketConnectionHandler.SendCentralSystemRequestAsync(context.Message.ChargePointId, messageOut);
    }
}