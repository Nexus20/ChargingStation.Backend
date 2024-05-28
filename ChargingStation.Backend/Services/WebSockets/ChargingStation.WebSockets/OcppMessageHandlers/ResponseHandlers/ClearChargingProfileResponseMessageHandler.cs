using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers.ResponseHandlers;

public class ClearChargingProfileResponseMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public ClearChargingProfileResponseMessageHandler(IConfiguration configuration, ILogger<ClearChargingProfileResponseMessageHandler> logger, IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }

    public sealed override string MessageType => Ocpp16ActionTypes.ClearChargingProfile;

    public sealed override bool IsResponseHandler => true;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing clear charging profile message...");
        var request = DeserializeMessage<ClearChargingProfileResponse>(inputMessage);
        Logger.LogTrace("ClearChargingProfile => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<ClearChargingProfileResponse>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}