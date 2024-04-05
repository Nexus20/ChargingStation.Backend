using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;

namespace ChargingStation.WebSockets.OcppMessageHandlers.Providers;

public interface IOcppMessageHandlerProvider
{
    public IOcppMessageHandler GetRequestHandler(string messageType, string protocolVersion);
    public IOcppMessageHandler GetResponseHandler(string messageType, string protocolVersion);
}