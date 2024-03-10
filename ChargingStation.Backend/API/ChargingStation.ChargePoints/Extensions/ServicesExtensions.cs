using System.Reflection;
using ChargingStation.ChargePoints.EventConsumers;
using ChargingStation.ChargePoints.Services;
using MassTransit;

namespace ChargingStation.ChargePoints.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargePointServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IChargePointService, ChargePointService>();
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<BootNotificationConsumer>();
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("boot-notification-queue", c => {
                    c.ConfigureConsumer<BootNotificationConsumer>(ctx);
                });
            });
            
            busConfigurator.AddConsumer<DataTransferConsumer>();
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("data-transfer-queue-1_6", c => {
                    c.ConfigureConsumer<DataTransferConsumer>(ctx);
                });
            });
        });

        return services;
    }
}
