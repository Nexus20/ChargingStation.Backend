using System.Reflection;
using ChargingStation.CacheManager.Extensions;
using ChargingStation.ChargingProfiles.EventConsumers;
using ChargingStation.ChargingProfiles.Services.ChargingProfiles;
using ChargingStation.ChargingProfiles.Services.EnergyConsumption;
using ChargingStation.InternalCommunication.Extensions;
using MassTransit;

namespace ChargingStation.ChargingProfiles.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargingProfilesServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        services.AddDepotsHttpClient(configuration);
        
        services.AddCacheServices(configuration);
        services.AddChargePointsGrpcClient(configuration);
        services.AddConnectorsGrpcClient(configuration);
        services.AddTransactionsGrpcClient(configuration);
        
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
        services.AddScoped<IEnergyConsumptionSettingsService, EnergyConsumptionSettingsService>();
        
        return services;
    }
}