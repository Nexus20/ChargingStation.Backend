using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;

namespace ChargingStation.WebSockets.OcppMessageHandlers.Providers;

public class OcppMessageHandlerProvider : IOcppMessageHandlerProvider
{
    private readonly IEnumerable<IOcppMessageHandler> _handlers;

    public OcppMessageHandlerProvider(IEnumerable<IOcppMessageHandler> handlers)
    {
        _handlers = handlers;
    }

    public IOcppMessageHandler GetRequestHandler(string messageType, string protocolVersion)
    {
        var handler =  _handlers.FirstOrDefault(x => x.MessageType == messageType && x.ProtocolVersion == protocolVersion);
        
        if (handler == null)
            throw new NotSupportedException($"No handler found for message type {messageType} and protocol version {protocolVersion}");
        
        return handler;
    }

    public IOcppMessageHandler GetResponseHandler(string messageType, string protocolVersion)
    {
        var handler =  _handlers.FirstOrDefault(x => x.MessageType == messageType && x.ProtocolVersion == protocolVersion && x.IsResponseHandler);
        
        if (handler == null)
            throw new NotSupportedException($"No response handler found for message type {messageType} and protocol version {protocolVersion}");
        
        return handler;
    }
}