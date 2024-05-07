using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OcppTags.Application.Services;

namespace OcppTags.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOcppTagApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IOcppTagService, OcppTagService>();
        
        return services;
    }
}