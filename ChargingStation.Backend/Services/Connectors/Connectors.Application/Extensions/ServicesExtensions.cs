using System.Reflection;
using Connectors.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Connectors.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddConnectorApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IConnectorService, ConnectorService>();
        
        return services;
    }
}