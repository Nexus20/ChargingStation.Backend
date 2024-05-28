using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using Connectors.Application.Services;
using MassTransit;

namespace Connectors.Api.EventConsumers;

public class ChangeAvailabilityResponseConsumer : IConsumer<IntegrationOcppMessage<ChangeAvailabilityResponse>>
{
    private readonly ILogger<ChangeAvailabilityResponseConsumer> _logger;
    private readonly IConnectorService _connectorService;

    public ChangeAvailabilityResponseConsumer(ILogger<ChangeAvailabilityResponseConsumer> logger, IConnectorService connectorService)
    {
        _logger = logger;
        _connectorService = connectorService;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<ChangeAvailabilityResponse>> context)
    {
        try
        {
            _logger.LogInformation("Processing change availability response message for charge point {ChargePointId}...", context.Message.ChargePointId);
        
            var incomingResponse = context.Message.Payload;
            var chargePointId = context.Message.ChargePointId;
            var ocppMessageId = context.Message.OcppMessageId;
        
            await _connectorService.ProcessChangeAvailabilityResponseAsync(incomingResponse, chargePointId, ocppMessageId, context.CancellationToken);
            _logger.LogInformation("Change availability response message processed for charge point {ChargePointId}", context.Message.ChargePointId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing change availability response message for charge point {ChargePointId}", context.Message.ChargePointId);
        }
    }
}