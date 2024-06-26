﻿using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers.RequestHandlers;

public class StartTransactionMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public StartTransactionMessageHandler(IConfiguration configuration, ILogger<StartTransactionMessageHandler> logger, IPublishEndpoint publishEndpoint) 
        : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public override string MessageType => Ocpp16ActionTypes.StartTransaction;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing start transaction message...");
        var request = DeserializeMessage<StartTransactionRequest>(inputMessage);
        Logger.LogTrace("StartTransaction => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<StartTransactionRequest>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}