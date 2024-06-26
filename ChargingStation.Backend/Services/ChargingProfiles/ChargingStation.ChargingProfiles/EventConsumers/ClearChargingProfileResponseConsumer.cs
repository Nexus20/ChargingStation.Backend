﻿using ChargingStation.ChargingProfiles.Services;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using MassTransit;

namespace ChargingStation.ChargingProfiles.EventConsumers;

public class ClearChargingProfileResponseConsumer : IConsumer<IntegrationOcppMessage<ClearChargingProfileResponse>>
{
    private readonly IChargingProfileService _chargingProfileService;
    private readonly ILogger<ClearChargingProfileResponseConsumer> _logger;

    public ClearChargingProfileResponseConsumer(IChargingProfileService chargingProfileService, ILogger<ClearChargingProfileResponseConsumer> logger)
    {
        _chargingProfileService = chargingProfileService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<ClearChargingProfileResponse>> context)
    {
        _logger.LogInformation("Processing clear charging profile response message...");
        
        var clearChargingProfileResponse = context.Message.Payload;
        var ocppMessageId = context.Message.OcppMessageId;
        var chargePointId = context.Message.ChargePointId;
        
        await _chargingProfileService.ProcessClearChargingProfileResponseAsync(clearChargingProfileResponse, chargePointId, ocppMessageId, context.CancellationToken);
    }
}