using ChargingStation.Common.Exceptions;

namespace ChargingStation.Reservations.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException exception)
        {
            logger.LogInformation(exception, "Resource not found");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (BadRequestException exception)
        {
            logger.LogInformation(exception, "Bad request occurred: {Message}", exception.Message);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        catch (Exception exception)
        {
            HandleStatus500Exception(context, exception, logger);
        }
    }

    private void HandleStatus500Exception(HttpContext context, Exception exception, ILogger logger)
    {
        logger.LogError(exception, "An exception was thrown as a result of the request");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    }
}