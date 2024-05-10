using EnergyConsumption.Application.Extensions;
using EnergyConsumption.Grpc.GrpcInterceptors;

namespace EnergyConsumption.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddEnergyConsumptionSettingsGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
            options.EnableDetailedErrors = true;
        });

        services.AddEnergyConsumptionSettingsApplicationServices(configuration);
        
        return services;
    }
}