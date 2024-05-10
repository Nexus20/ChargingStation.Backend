using Connectors.Api.EventConsumers;
using Connectors.Application.Extensions;
using MassTransit;

namespace Connectors.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddConnectorServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConnectorApplicationServices();

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