using ChargingStation.ChargePoints.Services;
using MassTransit;

namespace ChargingStation.ChargePoints.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargePointGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        
        services.AddAutoMapper(typeof(IChargePointService).Assembly);
        services.AddScoped<IChargePointService, ChargePointService>();
        
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
