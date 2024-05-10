using ChargingStation.Common.Constants;

namespace ChargingStation.WebSockets.OcppMessageHandlers.Abstract;

public abstract class Ocpp20MessageHandler : OcppMessageHandler
{
    protected Ocpp20MessageHandler(IConfiguration configuration, ILogger<Ocpp20MessageHandler> logger) : base(configuration, logger)
    {
    }

    public sealed override string ProtocolVersion => OcppProtocolVersions.Ocpp20;
}