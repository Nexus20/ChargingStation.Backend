using ChargingStation.Connectors.GrpcInterceptors;
using Connectors.Application.Extensions;
using MassTransit;

namespace Connectors.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddConnectorGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
            options.EnableDetailedErrors = true;
        });

        services.AddConnectorApplicationServices(configuration);

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
            });
        });
        
        return services;
    }
}