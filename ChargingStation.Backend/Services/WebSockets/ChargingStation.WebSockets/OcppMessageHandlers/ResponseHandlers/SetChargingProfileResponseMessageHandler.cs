using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers.ResponseHandlers;

public class SetChargingProfileResponseMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public SetChargingProfileResponseMessageHandler(IConfiguration configuration, ILogger<SetChargingProfileResponseMessageHandler> logger, IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }

    public sealed override string MessageType => Ocpp16ActionTypes.SetChargingProfile;

    public sealed override bool IsResponseHandler => true;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing set charging profile message...");
        var request = DeserializeMessage<SetChargingProfileResponse>(inputMessage);
        Logger.LogTrace("SetChargingProfile => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<SetChargingProfileResponse>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}