using System.Reflection;
using ChargingStation.Depots.Services;

namespace ChargingStation.Depots.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddDepotServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IDepotService, DepotService>();
        return services;
    }
}