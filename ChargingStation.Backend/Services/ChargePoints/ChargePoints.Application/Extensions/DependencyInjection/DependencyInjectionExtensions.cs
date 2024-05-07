using System.Reflection;
using ChargePoints.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChargePoints.Application.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddChargePointsApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IChargePointService, ChargePointService>();
        
        return services;
    }
}