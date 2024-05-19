using ChargingStation.Common.Configurations;
using Hangfire;
using MassTransit;
using Reservations.Api.EventConsumers;
using Reservations.Application.Extensions;

namespace Reservations.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddReservationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReservationApplicationServices(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<ReserveNowResponseConsumer>();
            busConfigurator.AddConsumer<CancelReservationResponseConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                var connectionString = configuration.GetSection(MessageBrokerConfiguration.SectionName).Get<MessageBrokerConfiguration>()!.GetConnectionString();
                cfg.Host(connectionString);
                
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