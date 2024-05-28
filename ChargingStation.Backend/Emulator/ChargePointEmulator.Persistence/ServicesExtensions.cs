using ChargePointEmulator.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChargePointEmulator.Persistence;

public static class ServicesExtensions
{
    public static IServiceCollection AddStateManagementRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IChargingStationStateRepository>(_ => new ChargingStationStateRepository(configuration));
        return services;
    }
}