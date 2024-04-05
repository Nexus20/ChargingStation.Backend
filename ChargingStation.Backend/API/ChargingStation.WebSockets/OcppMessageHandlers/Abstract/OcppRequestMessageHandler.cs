namespace ChargingStation.WebSockets.OcppMessageHandlers.Abstract;

public abstract class OcppRequestMessageHandler : OcppMessageHandler
{
    protected OcppRequestMessageHandler(IConfiguration configuration, ILogger<OcppRequestMessageHandler> logger) : base(configuration, logger)
    {
    }

    public sealed override bool IsResponseHandler => false;
}