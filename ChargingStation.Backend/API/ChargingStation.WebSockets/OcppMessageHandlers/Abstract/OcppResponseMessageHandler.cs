namespace ChargingStation.WebSockets.OcppMessageHandlers.Abstract;

public abstract class OcppResponseMessageHandler : OcppMessageHandler
{
    protected OcppResponseMessageHandler(IConfiguration configuration, ILogger<OcppResponseMessageHandler> logger) : base(configuration, logger)
    {
    }

    public sealed override bool IsResponseHandler => true;
}