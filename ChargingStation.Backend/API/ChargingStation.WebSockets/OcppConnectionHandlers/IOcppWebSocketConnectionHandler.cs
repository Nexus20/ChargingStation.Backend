using ChargingStation.Common.Models;

namespace ChargingStation.WebSockets.OcppConnectionHandlers;

public interface IOcppWebSocketConnectionHandler
{
    Task HandleConnectionAsync(RequestDelegate next, HttpContext context);
    Task SendResponseAsync(Guid chargePointId, OcppMessage messageOut);
}