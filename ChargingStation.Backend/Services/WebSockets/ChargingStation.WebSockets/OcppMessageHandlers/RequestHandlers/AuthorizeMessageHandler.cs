﻿using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers.RequestHandlers;

public class AuthorizeMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public AuthorizeMessageHandler(IConfiguration configuration, ILogger<AuthorizeMessageHandler> logger, IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }

    public override string MessageType => Ocpp16ActionTypes.Authorize;
    
    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing authorize message...");
        var request = DeserializeMessage<AuthorizeRequest>(inputMessage);
        Logger.LogTrace("Authorize => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<AuthorizeRequest>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}