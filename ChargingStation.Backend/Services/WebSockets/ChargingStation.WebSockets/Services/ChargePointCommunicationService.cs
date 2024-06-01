using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.InternalCommunication.GrpcClients;
using ChargingStation.WebSockets.OcppMessageHandlers.Providers;

namespace ChargingStation.WebSockets.Services;

public class ChargePointCommunicationService : IChargePointCommunicationService
{
    private readonly IOcppMessageHandlerProvider _ocppMessageHandlerProvider;
    private readonly ChargePointGrpcClientService _chargePointGrpcClientService;

    public ChargePointCommunicationService(IOcppMessageHandlerProvider ocppMessageHandlerProvider, ChargePointGrpcClientService chargePointGrpcClientService)
    {
        _ocppMessageHandlerProvider = ocppMessageHandlerProvider;
        _chargePointGrpcClientService = chargePointGrpcClientService;
    }

    public async Task CheckChargePointPresenceAsync(Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _chargePointGrpcClientService.GetByIdAsync(chargePointId, cancellationToken: cancellationToken);

        if (chargePoint is null)
            throw new NotFoundException(nameof(ChargePoint), chargePointId);
    }
    
    public async Task HandleMessageAsync(OcppMessage message, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var messageType = message.MessageType;
        var handler = _ocppMessageHandlerProvider.GetRequestHandler(messageType, "1.6");
        await handler.HandleAsync(message, chargePointId, cancellationToken);
    }
}