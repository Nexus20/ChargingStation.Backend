using System.Net;
using ChargingStation.Common.Exceptions;

namespace Connectors.Api.Middlewares;

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
            await HandleExceptionAsync(context, exception, StatusCodes.Status404NotFound);
        }
        catch (BadRequestException exception)
        {
            logger.LogInformation(exception, "Bad request occurred: {Message}", exception.Message);
            await HandleExceptionAsync(context, exception, StatusCodes.Status400BadRequest);
        }
        catch (UnauthorizedException exception)
        {
            logger.LogInformation(exception, "Unauthorized request");
            await HandleExceptionAsync(context, exception, StatusCodes.Status401Unauthorized);
        }
        catch (ForbiddenException exception)
        {
            logger.LogInformation(exception, "Forbidden request");
            await HandleExceptionAsync(context, exception, StatusCodes.Status403Forbidden);
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(exception, "An exception was thrown as a result of the request");
            await HandleExceptionAsync(context, exception, (int)(exception.StatusCode ?? HttpStatusCode.InternalServerError));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An exception was thrown as a result of the request");
            await HandleExceptionAsync(context, exception, StatusCodes.Status500InternalServerError);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(new ErrorDetails(exception).ToString());
    }
}