using EnergyConsumption.Application.Extensions;

namespace EnergyConsumption.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddEnergyConsumptionSettingsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEnergyConsumptionSettingsApplicationServices(configuration);
        
        return services;
    }
}