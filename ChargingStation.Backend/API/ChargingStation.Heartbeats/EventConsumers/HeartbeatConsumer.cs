using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models;
using ChargingStation.Common.Models.General;
using ChargingStation.Heartbeats.Services.Heartbeats;
using MassTransit;

namespace ChargingStation.Heartbeats.EventConsumers;

public class HeartbeatConsumer : IConsumer<IntegrationOcppMessage<HeartbeatRequest>>
{
    private readonly ILogger<HeartbeatConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IHeartbeatService _heartbeatService;

    public HeartbeatConsumer(ILogger<HeartbeatConsumer> logger, IPublishEndpoint publishEndpoint,
        IHeartbeatService heartbeatService)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _heartbeatService = heartbeatService;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<HeartbeatRequest>> context)
    {
        _logger.LogInformation("Received heartbeat request");

        var incomingRequest = context.Message.Payload;
        var chargePointId = context.Message.ChargePointId;
        var ocppProtocol = context.Message.OcppProtocol;

        var response =
            await _heartbeatService.ProcessHeartbeatAsync(incomingRequest, chargePointId, context.CancellationToken);

        var integrationMessage =
            ResponseIntegrationOcppMessage.Create(chargePointId, response, context.Message.OcppMessageId, ocppProtocol);
        await _publishEndpoint.Publish(integrationMessage, context.CancellationToken);

        _logger.LogInformation("Heartbeat request processed and response published");
    }
}