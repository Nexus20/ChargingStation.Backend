using System.Reflection;
using ChargingStation.CacheManager.Extensions;
using Connectors.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Connectors.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddConnectorApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IConnectorService, ConnectorService>();
        services.AddCacheServices(configuration);
        
        return services;
    }
}