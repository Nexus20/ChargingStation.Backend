using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Models;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers;

public class MeterValuesMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public MeterValuesMessageHandler(IConfiguration configuration, ILogger<MeterValuesMessageHandler> logger, IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public override string MessageType => Ocpp16MessageTypes.MeterValues;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing meter values message...");
        var request = DeserializeMessage<MeterValuesRequest>(inputMessage);
        Logger.LogTrace("MeterValues => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<MeterValuesRequest>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}