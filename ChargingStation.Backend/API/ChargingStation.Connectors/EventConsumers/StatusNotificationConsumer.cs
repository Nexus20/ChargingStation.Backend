using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models;
using ChargingStation.Common.Models.General;
using ChargingStation.Connectors.Services;
using MassTransit;

namespace ChargingStation.Connectors.EventConsumers;

public class StatusNotificationConsumer : IConsumer<IntegrationOcppMessage<StatusNotificationRequest>>
{
    private readonly ILogger<StatusNotificationConsumer> _logger;
    private readonly IConnectorService _connectorService;
    private readonly IPublishEndpoint _publishEndpoint;

    public StatusNotificationConsumer(ILogger<StatusNotificationConsumer> logger, IConnectorService connectorService, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _connectorService = connectorService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<StatusNotificationRequest>> context)
    {
        _logger.LogInformation("Processing start transaction message...");
        
        var incomingRequest = context.Message.Payload;
        var chargePointId = context.Message.ChargePointId;
        var ocppProtocol = context.Message.OcppProtocol;

        var response = await _connectorService.ProcessStatusNotificationAsync(incomingRequest, chargePointId, context.CancellationToken);
        
        var integrationMessage = ResponseIntegrationOcppMessage.Create(chargePointId, response, context.Message.OcppMessageId, ocppProtocol);
        await _publishEndpoint.Publish(integrationMessage, context.CancellationToken);
        
        _logger.LogInformation("Start transaction message processed");
    }
}