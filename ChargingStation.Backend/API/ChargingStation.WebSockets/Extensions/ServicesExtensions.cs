using ChargingStation.WebSockets.EventConsumers;
using ChargingStation.WebSockets.OcppConnectionHandlers;
using ChargingStation.WebSockets.OcppMessageHandlers;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using ChargingStation.WebSockets.OcppMessageHandlers.Providers;
using ChargingStation.WebSockets.Services;
using MassTransit;

namespace ChargingStation.WebSockets.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOcppCommunicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IChargePointCommunicationService, ChargePointCommunicationService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:ChargePointServiceAddress"]!);
        });

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<OcppResponseConsumer>();
            busConfigurator.AddConsumer<ResetConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("ocpp-response-queue", c => {
                    c.ConfigureConsumer<OcppResponseConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("reset-queue", c => {
                    c.ConfigureConsumer<ResetConsumer>(ctx);
                });
            });
        });

        services.AddScoped<IOcppMessageHandler, BootNotificationMessageHandler>();
        services.AddScoped<IOcppMessageHandler, AuthorizeMessageHandler>();
        services.AddScoped<IOcppMessageHandler, DataTransferMessageHandler>();
        services.AddScoped<IOcppMessageHandler, MeterValuesMessageHandler>();
        services.AddScoped<IOcppMessageHandler, StartTransactionMessageHandler>();
        services.AddScoped<IOcppMessageHandler, StatusNotificationMessageHandler>();
        services.AddScoped<IOcppMessageHandler, StopTransactionMessageHandler>();
        services.AddScoped<IOcppMessageHandler, HeartbeatMessageHandler>();
        
        services.AddScoped<IOcppMessageHandlerProvider, OcppMessageHandlerProvider>();
        services.AddScoped<IOcppWebSocketConnectionHandler, OcppWebSocketConnectionHandler>();

        return services;
    }
}
