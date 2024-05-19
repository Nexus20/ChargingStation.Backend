using ChargingStation.Common.Configurations;
using Connectors.Application.Extensions;
using Connectors.Grpc.GrpcInterceptors;
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

        services.AddConnectorApplicationServices();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                var connectionString = configuration.GetSection(MessageBrokerConfiguration.SectionName).Get<MessageBrokerConfiguration>()!.GetConnectionString();
                cfg.Host(connectionString);
            });
        });
        
        return services;
    }
}