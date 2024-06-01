using System.Reflection;
using Depots.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Depots.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddDepotApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IDepotService, DepotService>();

        return services;
    }
}