using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers.ResponseHandlers;

public class CancelReservationResponseMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public CancelReservationResponseMessageHandler(IConfiguration configuration, ILogger<CancelReservationResponseMessageHandler> logger, IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }

    public sealed override string MessageType => Ocpp16ActionTypes.CancelReservation;

    public sealed override bool IsResponseHandler => true;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing cancel reservation message...");
        var request = DeserializeMessage<CancelReservationResponse>(inputMessage);
        Logger.LogTrace("CancelReservation => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<CancelReservationResponse>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}