using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Models;
using ChargingStation.OcppTags.Helpers;
using ChargingStation.OcppTags.Services;
using MassTransit;

namespace ChargingStation.OcppTags.EventConsumers;

public class AuthorizeConsumer : IConsumer<IntegrationOcppMessage<AuthorizeRequest>>
{
    private readonly ILogger<AuthorizeConsumer> _logger;
    private readonly IOcppTagService _ocppTagService;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthorizeConsumer(ILogger<AuthorizeConsumer> logger, IOcppTagService ocppTagService, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _ocppTagService = ocppTagService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IntegrationOcppMessage<AuthorizeRequest>> context)
    {
        _logger.LogInformation("Processing authorize message...");
        
        var incomingRequest = context.Message.Payload;
        var chargePointId = context.Message.ChargePointId;
        var ocppProtocol = context.Message.OcppProtocol;
        
        var response = new AuthorizeResponse();
        var idTag = OcppTagHelper.CleanChargeTagId(incomingRequest.IdTag, _logger);
        response.IdTagInfo.ExpiryDate = DateTimeOffset.UtcNow.AddMinutes(5);   // default: 5 minutes
        
        var ocppTag = await _ocppTagService.GetByOcppTagIdAsync(idTag);
        
        if (ocppTag != null)
        {
            if (ocppTag.ExpiryDate.HasValue) 
                response.IdTagInfo.ExpiryDate = ocppTag.ExpiryDate.Value;
            
            response.IdTagInfo.ParentIdTag = ocppTag.ParentTagId ?? string.Empty;
            
            if (ocppTag.Blocked.HasValue && ocppTag.Blocked.Value)
                response.IdTagInfo.Status = IdTagInfoStatus.Blocked;
            
            else if (ocppTag.ExpiryDate.HasValue && ocppTag.ExpiryDate.Value < DateTime.Now)
                response.IdTagInfo.Status = IdTagInfoStatus.Expired;
            
            else
                response.IdTagInfo.Status = IdTagInfoStatus.Accepted;
        }
        else
            response.IdTagInfo.Status = IdTagInfoStatus.Invalid;
        
        var integrationMessage = ResponseIntegrationOcppMessage.Create(chargePointId, response, context.Message.OcppMessageId, ocppProtocol);
        await _publishEndpoint.Publish(integrationMessage, context.CancellationToken);
        
        _logger.LogInformation("Authorize message processed");
    }
}