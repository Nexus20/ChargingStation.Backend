using ChargePoints.Api.EventConsumers;
using ChargePoints.Application.Extensions.DependencyInjection;
using MassTransit;

namespace ChargePoints.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargePointServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddChargePointsApplicationServices();
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<BootNotificationConsumer>();
            busConfigurator.AddConsumer<DataTransferConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("boot-notification-queue", c => {
                    c.ConfigureConsumer<BootNotificationConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("data-transfer-queue-1_6", c => {
                    c.ConfigureConsumer<DataTransferConsumer>(ctx);
                });
            });
        });

        return services;
    }
}
