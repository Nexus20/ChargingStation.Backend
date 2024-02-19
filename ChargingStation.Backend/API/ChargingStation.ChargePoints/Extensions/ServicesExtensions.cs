using System.Reflection;
using ChargingStation.ChargePoints.Services;

namespace ChargingStation.ChargePoints.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargePointServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IChargePointService, ChargePointService>();

        return services;
    }
}
