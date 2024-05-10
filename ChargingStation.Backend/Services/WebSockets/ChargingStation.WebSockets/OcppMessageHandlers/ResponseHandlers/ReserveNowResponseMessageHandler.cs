using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers.ResponseHandlers;

public class ReserveNowResponseMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public ReserveNowResponseMessageHandler(IConfiguration configuration, ILogger<ReserveNowResponseMessageHandler> logger, IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }

    public sealed override string MessageType => Ocpp16ActionTypes.ReserveNow;

    public sealed override bool IsResponseHandler => true;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing stop transaction message...");
        var request = DeserializeMessage<ReserveNowResponse>(inputMessage);
        Logger.LogTrace("StopTransaction => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<ReserveNowResponse>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}