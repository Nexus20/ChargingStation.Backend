﻿using ChargingStation.Common.Configurations;
using Hangfire;
using MassTransit;
using Reservations.Application.Extensions;

namespace Reservations.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddReservationGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        
        services.AddReservationApplicationServices(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                var connectionString = configuration.GetSection(MessageBrokerConfiguration.SectionName).Get<MessageBrokerConfiguration>()!.GetConnectionString();
                cfg.Host(connectionString);
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