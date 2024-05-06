using System.Reflection;
using ChargingStation.Reservations.EventConsumers;
using ChargingStation.Reservations.Services.Reservations;
using Hangfire;
using MassTransit;

namespace ChargingStation.Reservations.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddReservationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IReservationService, ReservationService>();
        
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

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"))
        );
        
        services.AddHangfireServer();

        return services;
    }
}