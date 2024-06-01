using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Depots.Grpc.GrpcInterceptors;

public class GrpcExceptionInterceptor : Interceptor
{
    private readonly ILogger _logger;

    public GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("Starting receiving call. Type/Method: {Type} / {Method}",
            MethodType.Unary, context.Method);
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            // TODO: Add exception handling
            _logger.LogError(ex, "Error thrown by {Method}", context.Method);
            throw;
        }
    }
}