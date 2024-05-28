using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers.ResponseHandlers;

public class ChangeAvailabilityResponseMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public ChangeAvailabilityResponseMessageHandler(IConfiguration configuration, ILogger<ChangeAvailabilityResponseMessageHandler> logger, IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }

    public sealed override string MessageType => Ocpp16ActionTypes.ChangeAvailability;

    public sealed override bool IsResponseHandler => true;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing change availability message...");
        var request = DeserializeMessage<ChangeAvailabilityResponse>(inputMessage);
        Logger.LogTrace("ChangeAvailability => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<ChangeAvailabilityResponse>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}