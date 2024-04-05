using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models;
using ChargingStation.Common.Models.General;

namespace ChargingStation.WebSockets.OcppConnectionHandlers;

public interface IOcppWebSocketConnectionHandler
{
    Task HandleConnectionAsync(RequestDelegate next, HttpContext context);
    Task SendResponseAsync(Guid chargePointId, OcppMessage messageOut);
    Task SendCentralSystemRequestAsync(Guid chargePointId, OcppMessage centralSystemRequest, CancellationToken cancellationToken = default);
    Task SendResetAsync(Guid chargePointId, ResetRequest incomingRequest);
}