using ChargingStation.WebSockets.OcppConnectionHandlers;

namespace ChargingStation.WebSockets.Middlewares;

public class OcppWebSocketMiddleware
{
    private readonly RequestDelegate _next;

    public OcppWebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IOcppWebSocketConnectionHandler ocppWebSocketConnectionHandler, 
        ILogger<OcppWebSocketMiddleware> logger)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            await ocppWebSocketConnectionHandler.HandleConnectionAsync(_next, context);
            return;
        }
        
        // passed on to next middleware
        await _next(context);
    }
}