using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using MassTransit;

namespace ChargingStation.WebSockets.OcppMessageHandlers.RequestHandlers;

public class MeterValuesMessageHandler : Ocpp16MessageHandler
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public MeterValuesMessageHandler(IConfiguration configuration, ILogger<MeterValuesMessageHandler> logger, IPublishEndpoint publishEndpoint) : base(configuration, logger)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public override string MessageType => Ocpp16ActionTypes.MeterValues;

    public override async Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Processing meter values message...");
        var request = DeserializeMessage<MeterValuesRequest>(inputMessage);
        Logger.LogTrace("MeterValues => Message deserialized");
        var integrationMessage = new IntegrationOcppMessage<MeterValuesRequest>(chargePointId, request, inputMessage.UniqueId, ProtocolVersion);
        await _publishEndpoint.Publish(integrationMessage, cancellationToken);
    }
}