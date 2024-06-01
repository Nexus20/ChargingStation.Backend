using Depots.Application.Extensions;
using Depots.Grpc.GrpcInterceptors;

namespace Depots.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddDepotGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
            options.EnableDetailedErrors = true;
        });

        services.AddDepotApplicationServices();
        
        return services;
    }
}