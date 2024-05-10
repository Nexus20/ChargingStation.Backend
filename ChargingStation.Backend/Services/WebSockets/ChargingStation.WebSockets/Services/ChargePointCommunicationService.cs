using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.WebSockets.OcppMessageHandlers.Providers;

namespace ChargingStation.WebSockets.Services;

public class ChargePointCommunicationService : IChargePointCommunicationService
{
    private readonly HttpClient _httpClient;
    private readonly IOcppMessageHandlerProvider _ocppMessageHandlerProvider;

    public ChargePointCommunicationService(HttpClient httpClient, IOcppMessageHandlerProvider ocppMessageHandlerProvider)
    {
        _httpClient = httpClient;
        _ocppMessageHandlerProvider = ocppMessageHandlerProvider;
    }

    public async Task CheckChargePointPresenceAsync(Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"api/ChargePoint/{chargePointId}";
        var result = await _httpClient.GetAsync(requestUri, cancellationToken);

        if (!result.IsSuccessStatusCode)
            throw new NotFoundException(nameof(ChargePoint), chargePointId);
    }
    
    public async Task HandleMessageAsync(OcppMessage message, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var messageType = message.MessageType;
        var handler = _ocppMessageHandlerProvider.GetRequestHandler(messageType, "1.6");
        await handler.HandleAsync(message, chargePointId, cancellationToken);
    }
}