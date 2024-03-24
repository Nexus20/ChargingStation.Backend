using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Transactions.Services.MeterValues;
using MassTransit;

namespace ChargingStation.Transactions.EventConsumers;

public class MeterValueConsumer : IConsumer<IntegrationOcppMessage<MeterValuesRequest>>
{
    private readonly ILogger<MeterValueConsumer> _logger;
    private readonly IMeterValueService _meterValueService;
    private readonly IPublishEndpoint _publishEndpoint;

    public MeterValueConsumer(ILogger<MeterValueConsumer> logger, IMeterValueService meterValueService, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _meterValueService = meterValueService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<MeterValuesRequest>> context)
    {
        _logger.LogInformation("Processing meter value message...");
        
        var incomingRequest = context.Message.Payload;
        var chargePointId = context.Message.ChargePointId;
        var ocppProtocol = context.Message.OcppProtocol;

        var response = await _meterValueService.ProcessMeterValueAsync(incomingRequest, chargePointId, context.CancellationToken);
        
        var integrationMessage = ResponseIntegrationOcppMessage.Create(chargePointId, response, context.Message.OcppMessageId, ocppProtocol);
        await _publishEndpoint.Publish(integrationMessage, context.CancellationToken);
        
        _logger.LogInformation("Meter value message processed");
    }
}