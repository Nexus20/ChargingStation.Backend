using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;

namespace ChargingStation.WebSockets.OcppMessageHandlers.Providers;

public interface IOcppMessageHandlerProvider
{
    public IOcppMessageHandler GetHandler(string messageType, string protocolVersion);
}