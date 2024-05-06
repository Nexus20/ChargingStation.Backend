using ChargingStation.InternalCommunication.Extensions;
using ChargingStation.Reservations.Services.Reservations;
using Hangfire;
using MassTransit;

namespace ChargingStation.Reservations.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddReservationGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        
        services.AddChargePointsGrpcClient(configuration);
        services.AddOcppTagsGrpcClient(configuration);
        services.AddConnectorsGrpcClient(configuration);
        
        services.AddAutoMapper(typeof(IReservationService).Assembly);

        services.AddScoped<IReservationService, ReservationService>();
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
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