using System.Reflection;
using ChargingStation.InternalCommunication.Extensions;
using Depots.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Depots.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddDepotApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUsersHttpClient(configuration);
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IDepotService, DepotService>();

        return services;
    }
}