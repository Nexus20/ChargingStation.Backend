using ChargePoints.Application.Models.Requests;
using ChargePoints.Application.Services;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using ChargingStation.Common.Models.General;
using ChargingStation.InternalCommunication.SignalRModels;
using MassTransit;
using Newtonsoft.Json;

namespace ChargePoints.Api.EventConsumers;

public class BootNotificationConsumer : IConsumer<IntegrationOcppMessage<BootNotificationRequest>>
{
    private readonly ILogger<BootNotificationConsumer> _logger;
    private readonly IChargePointService _chargePointService;
    private readonly IPublishEndpoint _publishEndpoint;

    public BootNotificationConsumer(ILogger<BootNotificationConsumer> logger, IChargePointService chargePointService, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _chargePointService = chargePointService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<BootNotificationRequest>> context)
    {
        _logger.LogInformation("Processing boot notification...");
        
        var incomingRequest = context.Message.Payload;
        var chargePointId = context.Message.ChargePointId;
        var ocppProtocol = context.Message.OcppProtocol;
        
        var chargePoint = await _chargePointService.GetByIdAsync(chargePointId);

        var updateRequest = new UpdateChargePointRequest
        {
            Id = chargePointId,
            DepotId = chargePoint.DepotId,
            OcppProtocol = ocppProtocol,
            ChargePointVendor = incomingRequest.ChargePointVendor,
            ChargePointModel = incomingRequest.ChargePointModel,
            ChargeBoxSerialNumber = incomingRequest.ChargeBoxSerialNumber,
            FirmwareVersion = incomingRequest.FirmwareVersion,
            Iccid = incomingRequest.Iccid,
            Imsi = incomingRequest.Imsi,
            MeterType = incomingRequest.MeterType,
            MeterSerialNumber = incomingRequest.MeterSerialNumber,
            ChargePointSerialNumber = incomingRequest.ChargePointSerialNumber,
            Name = string.IsNullOrEmpty(chargePoint.Name) ? $"chargePoint{chargePoint.Id}" : chargePoint.Name
        };
        
        await _chargePointService.UpdateAsync(updateRequest);

        var response = new BootNotificationResponse(BootNotificationResponseStatus.Accepted, DateTimeOffset.UtcNow, 60);
        
        var integrationMessage = CentralSystemResponseIntegrationOcppMessage.Create(chargePointId, response, context.Message.OcppMessageId, ocppProtocol);

        var stationConnectionMessage = new StationConnectionMessage()
        {
            ChargePointId = chargePointId
        };
        var signalRMessage = new SignalRMessage(JsonConvert.SerializeObject(stationConnectionMessage), nameof(stationConnectionMessage));

        await _publishEndpoint.Publish(integrationMessage, context.CancellationToken);
        await _publishEndpoint.Publish(signalRMessage, context.CancellationToken);

        _logger.LogInformation("BootNotificationConsumer consumed successfully. ChargePointId : {ChargePointId}", context.Message.ChargePointId);
    }
}