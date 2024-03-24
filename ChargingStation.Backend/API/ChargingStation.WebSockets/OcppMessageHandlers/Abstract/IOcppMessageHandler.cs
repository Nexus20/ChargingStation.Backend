using ChargingStation.Common.Models;
using ChargingStation.Common.Models.General;

namespace ChargingStation.WebSockets.OcppMessageHandlers.Abstract;

public interface IOcppMessageHandler
{
    public string ProtocolVersion { get; }
    public string MessageType { get; }
    
    public Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default);
}