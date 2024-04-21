using ChargingStation.SignalR.EventConsumers;
using ChargingStation.SignalR.Hubs;
using MassTransit;

namespace ChargingStation.SignalR.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddSignalRServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<HubFacade>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<SignalRResponseConsumer>();

            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);

                cfg.ReceiveEndpoint("signalR-queue", c => {
                    c.ConfigureConsumer<SignalRResponseConsumer>(ctx);
                });
            });
        });

        return services;
    }
}