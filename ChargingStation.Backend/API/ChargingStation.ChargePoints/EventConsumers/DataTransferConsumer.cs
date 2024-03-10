using ChargingStation.ChargePoints.Services;
using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Models;
using MassTransit;

namespace ChargingStation.ChargePoints.EventConsumers;

public class DataTransferConsumer : IConsumer<IntegrationOcppMessage<DataTransferRequest>>
{
    private readonly ILogger<DataTransferConsumer> _logger;
    private readonly IChargePointService _chargePointService;
    private readonly IPublishEndpoint _publishEndpoint;

    public DataTransferConsumer(ILogger<DataTransferConsumer> logger, IChargePointService chargePointService, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _chargePointService = chargePointService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<DataTransferRequest>> context)
    {
        try
        {

            _logger.LogInformation("Processing data transfer message...");

            var incomingRequest = context.Message.Payload;
            var chargePointId = context.Message.ChargePointId;
            var ocppProtocol = context.Message.OcppProtocol;
            
            _logger.LogInformation("Data transfer message data {RequestData}. Vendor Id {VendorId}", incomingRequest.Data, incomingRequest.VendorId);

            var chargePoint = await _chargePointService.GetByIdAsync(chargePointId);

            var response = new DataTransferResponse
            {
                Status = DataTransferResponseStatus.Accepted,
                Data = "Data transfer accepted"
            };

            var integrationMessage = ResponseIntegrationOcppMessage.Create(chargePointId, response,
                context.Message.OcppMessageId, ocppProtocol);

            await _publishEndpoint.Publish(integrationMessage, context.CancellationToken);
            _logger.LogInformation("Data transfer message processed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing data transfer message");
        }
    }
}