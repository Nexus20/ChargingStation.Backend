using ChargePoints.Application.Extensions.DependencyInjection;
using MassTransit;

namespace ChargePoints.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargePointGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        services.AddChargePointsApplicationServices();
        
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
