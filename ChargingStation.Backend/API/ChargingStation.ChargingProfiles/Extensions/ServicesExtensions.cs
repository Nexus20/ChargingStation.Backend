using System.Reflection;
using ChargingStation.CacheManager.Extensions;
using ChargingStation.ChargingProfiles.EventConsumers;
using ChargingStation.ChargingProfiles.Services;
using ChargingStation.Infrastructure;
using ChargingStation.InternalCommunication.Extensions;
using MassTransit;

namespace ChargingStation.ChargingProfiles.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargingProfilesServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        services.AddInfrastructureServices(configuration);
        services.AddCacheServices(configuration);
        services.AddConnectorsHttpClient(configuration);
        services.AddTransactionsHttpClient(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<SetChargingProfileResponseConsumer>();
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("set-charging-profile-response-queue-1_6", c => {
                    c.ConfigureConsumer<SetChargingProfileResponseConsumer>(ctx);
                });
            });
        });
        
        services.AddScoped<IChargingProfileService, ChargingProfileService>();
        
        return services;
    }
}