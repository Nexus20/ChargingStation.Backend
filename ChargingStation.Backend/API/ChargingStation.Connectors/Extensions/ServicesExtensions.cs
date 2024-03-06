using System.Reflection;
using ChargingStation.Connectors.EventConsumers;
using ChargingStation.Connectors.Services;
using MassTransit;

namespace ChargingStation.Connectors.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddConnectorServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IConnectorService, ConnectorService>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<StatusNotificationConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("status-notification-queue-1_6", c => {
                    c.ConfigureConsumer<StatusNotificationConsumer>(ctx);
                });
            });
        });
        
        return services;
    }
}