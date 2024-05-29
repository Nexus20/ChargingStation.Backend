using ChargingStation.Common.Configurations;
using ChargingStation.WebSockets.EventConsumers;
using ChargingStation.WebSockets.OcppConnectionHandlers;
using ChargingStation.WebSockets.OcppMessageHandlers.Abstract;
using ChargingStation.WebSockets.OcppMessageHandlers.Providers;
using ChargingStation.WebSockets.OcppMessageHandlers.RequestHandlers;
using ChargingStation.WebSockets.OcppMessageHandlers.ResponseHandlers;
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

            busConfigurator.AddConsumer<OcppCentralSystemRequestConsumer>();
            busConfigurator.AddConsumer<OcppCentralSystemResponseConsumer>();
            busConfigurator.AddConsumer<ResetConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                var connectionString = configuration.GetSection(MessageBrokerConfiguration.SectionName).Get<MessageBrokerConfiguration>()!.GetConnectionString();
                cfg.Host(connectionString);
                
                cfg.ReceiveEndpoint("ocpp-request-queue", c => {
                    c.ConfigureConsumer<OcppCentralSystemRequestConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("ocpp-response-queue", c => {
                    c.ConfigureConsumer<OcppCentralSystemResponseConsumer>(ctx);
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
        
        services.AddScoped<IOcppMessageHandler, ReserveNowResponseMessageHandler>();
        services.AddScoped<IOcppMessageHandler, CancelReservationResponseMessageHandler>();
        services.AddScoped<IOcppMessageHandler, SetChargingProfileResponseMessageHandler>();
        services.AddScoped<IOcppMessageHandler, ClearChargingProfileResponseMessageHandler>();
        services.AddScoped<IOcppMessageHandler, ChangeAvailabilityResponseMessageHandler>();
        
        services.AddScoped<IOcppMessageHandlerProvider, OcppMessageHandlerProvider>();
        services.AddScoped<IOcppWebSocketConnectionHandler, OcppWebSocketConnectionHandler>();

        return services;
    }
}
