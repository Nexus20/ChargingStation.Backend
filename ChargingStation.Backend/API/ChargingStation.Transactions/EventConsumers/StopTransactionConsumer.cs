using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Models;
using ChargingStation.Transactions.Services.Transactions;
using MassTransit;

namespace ChargingStation.Transactions.EventConsumers;

public class StopTransactionConsumer : IConsumer<IntegrationOcppMessage<StopTransactionRequest>>
{
    private readonly ILogger<StopTransactionConsumer> _logger;
    private readonly ITransactionService _transactionService;
    private readonly IPublishEndpoint _publishEndpoint;

    public StopTransactionConsumer(ILogger<StopTransactionConsumer> logger, ITransactionService transactionService, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _transactionService = transactionService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<StopTransactionRequest>> context)
    {
        _logger.LogInformation("Processing stop transaction message...");
        
        var incomingRequest = context.Message.Payload;
        var chargePointId = context.Message.ChargePointId;
        var ocppProtocol = context.Message.OcppProtocol;

        var response = await _transactionService.ProcessStopTransactionAsync(incomingRequest, chargePointId, context.CancellationToken);
        
        var integrationMessage = ResponseIntegrationOcppMessage.Create(chargePointId, response, context.Message.OcppMessageId, ocppProtocol);
        await _publishEndpoint.Publish(integrationMessage, context.CancellationToken);
        
        _logger.LogInformation("Stop transaction message processed");
    }
}