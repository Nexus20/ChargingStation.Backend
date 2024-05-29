using ChargePoints.Application.Extensions.DependencyInjection;
using ChargingStation.Common.Configurations;
using MassTransit;

namespace ChargePoints.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargePointGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        services.AddChargePointsApplicationServices(configuration);
        
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
