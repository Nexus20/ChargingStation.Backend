using ChargingStation.Connectors.GrpcInterceptors;
using ChargingStation.Connectors.Services;
using MassTransit;

namespace ChargingStation.Connectors.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddConnectorGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
            options.EnableDetailedErrors = true;
        });
        
        services.AddAutoMapper(typeof(IConnectorService).Assembly);
        services.AddScoped<IConnectorService, ConnectorService>();

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