using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models;

namespace ChargingStation.WebSockets.OcppConnectionHandlers;

public interface IOcppWebSocketConnectionHandler
{
    Task HandleConnectionAsync(RequestDelegate next, HttpContext context);
    Task SendResponseAsync(Guid chargePointId, OcppMessage messageOut);
    Task SendResetAsync(Guid chargePointId, ResetRequest incomingRequest);
}