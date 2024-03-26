using System.Reflection;
using ChargingStation.InternalCommunication.Extensions;
using ChargingStation.Reservations.EventConsumers;
using ChargingStation.Reservations.Services.Reservations;
using MassTransit;

namespace ChargingStation.Reservations.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddReservationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IReservationService, ReservationService>();
        
        services.AddOcppTagsHttpClient(configuration);
        services.AddChargePointsHttpClient(configuration);
        services.AddConnectorsHttpClient(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<ReserveNowResponseConsumer>();
            busConfigurator.AddConsumer<CancelReservationResponseConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("reserve-now-response-queue-1_6", c => {
                    c.ConfigureConsumer<ReserveNowResponseConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("cancel-reservation-response-queue-1_6", c => {
                    c.ConfigureConsumer<CancelReservationResponseConsumer>(ctx);
                });
            });
        });

        return services;
    }
}