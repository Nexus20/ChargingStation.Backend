using System.Reflection;
using ChargingStation.InternalCommunication.Extensions;
using EnergyConsumption.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnergyConsumption.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddEnergyConsumptionSettingsApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddDepotsHttpClient(configuration);
        services.AddChargePointsGrpcClient(configuration);
        
        services.AddScoped<IEnergyConsumptionSettingsService, EnergyConsumptionSettingsService>();
        
        return services;
    }
}