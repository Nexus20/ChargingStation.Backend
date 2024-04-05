using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers.RequestHandlers;

public class HeartbeatMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;

    public HeartbeatMessageHandler(IConfiguration configuration, ILogger<HeartbeatMessageHandler> logger,
        IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }

    public override string MessageType => Ocpp16ActionTypes.Heartbeat;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId,
        CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing heartbeat...");
        var request = DeserializeMessage<HeartbeatRequest>(inputMessage);
        Logger.LogTrace("Heartbeat => Message deserialized");
        var integrationMessage =
            new IntegrationOcppMessage<HeartbeatRequest>(chargePointId, request, inputMessage.UniqueId,
                ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}