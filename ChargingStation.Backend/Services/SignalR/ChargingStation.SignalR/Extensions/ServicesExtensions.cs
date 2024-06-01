using ChargingStation.Common.Configurations;
using ChargingStation.SignalR.EventConsumers;
using ChargingStation.SignalR.Hubs;
using MassTransit;

namespace ChargingStation.SignalR.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddSignalRServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();
        services.AddSingleton<HubFacade>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<SignalRResponseConsumer>();

            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                var connectionString = configuration.GetSection(MessageBrokerConfiguration.SectionName).Get<MessageBrokerConfiguration>()!.GetConnectionString();
                cfg.Host(connectionString);

                cfg.ReceiveEndpoint("signalR-queue", c => {
                    c.ConfigureConsumer<SignalRResponseConsumer>(ctx);
                });
            });
        });

        return services;
    }
}