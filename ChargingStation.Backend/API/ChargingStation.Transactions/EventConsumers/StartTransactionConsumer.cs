using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Transactions.Services.Transactions;
using MassTransit;

namespace ChargingStation.Transactions.EventConsumers;

public class StartTransactionConsumer : IConsumer<IntegrationOcppMessage<StartTransactionRequest>>
{
    private readonly ILogger<StartTransactionConsumer> _logger;
    private readonly ITransactionService _transactionService;
    private readonly IPublishEndpoint _publishEndpoint;

    public StartTransactionConsumer(ILogger<StartTransactionConsumer> logger, ITransactionService transactionService, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _transactionService = transactionService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<StartTransactionRequest>> context)
    {
        _logger.LogInformation("Processing start transaction message...");
        
        var incomingRequest = context.Message.Payload;
        var chargePointId = context.Message.ChargePointId;
        var ocppProtocol = context.Message.OcppProtocol;

        var response = await _transactionService.ProcessStartTransactionAsync(incomingRequest, chargePointId, context.CancellationToken);
        
        var integrationMessage = ResponseIntegrationOcppMessage.Create(chargePointId, response, context.Message.OcppMessageId, ocppProtocol);
        await _publishEndpoint.Publish(integrationMessage, context.CancellationToken);
        
        _logger.LogInformation("Start transaction message processed");
    }
}