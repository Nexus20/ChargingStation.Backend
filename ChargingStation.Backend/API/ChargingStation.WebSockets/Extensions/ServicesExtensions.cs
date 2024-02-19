using ChargingStation.WebSockets.EventConsumers;
using ChargingStation.WebSockets.Middlewares;
using ChargingStation.WebSockets.OcppConnectionHandlers;
using ChargingStation.WebSockets.OcppMessageHandlers;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using ChargingStation.WebSockets.OcppMessageHandlers.Providers;
using ChargingStation.WebSockets.Services;
using MassTransit;

namespace ChargingStation.WebSockets.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargePointServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IChargePointCommunicationService, ChargePointCommunicationService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:ChargingStationServiceAddress"]!);
        });

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<OcppResponseConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("ocpp-response-queue", c => {
                    c.ConfigureConsumer<OcppResponseConsumer>(ctx);
                });
            });
        });

        services.AddScoped<IOcppMessageHandler, BootNotificationMessageHandler>();
        services.AddScoped<IOcppMessageHandlerProvider, OcppMessageHandlerProvider>();
        services.AddScoped<IOcppWebSocketConnectionHandler, OcppWebSocketConnectionHandler>();

        return services;
    }
}
