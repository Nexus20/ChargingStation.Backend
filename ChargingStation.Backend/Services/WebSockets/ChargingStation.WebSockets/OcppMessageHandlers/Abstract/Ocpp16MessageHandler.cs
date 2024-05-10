using ChargingStation.Common.Constants;

namespace ChargingStation.WebSockets.OcppMessageHandlers.Abstract;

public abstract class Ocpp16MessageHandler : OcppMessageHandler
{
    protected Ocpp16MessageHandler(IConfiguration configuration, ILogger<Ocpp16MessageHandler> logger) : base(configuration, logger)
    {
    }

    public sealed override string ProtocolVersion => OcppProtocolVersions.Ocpp16;
}