using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Models;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers;

public class BootNotificationMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public BootNotificationMessageHandler(IConfiguration configuration, ILogger<BootNotificationMessageHandler> logger, IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public override string MessageType => Ocpp16MessageTypes.BootNotification;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing boot notification...");
        var request = DeserializeMessage<BootNotificationRequest>(inputMessage);
        Logger.LogTrace("BootNotification => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<BootNotificationRequest>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}