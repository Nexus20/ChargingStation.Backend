using System.Reflection;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Azure.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WineQuality.Application.Interfaces.FileStorage;
using WineQuality.Application.Interfaces.Infrastructure;
using WineQuality.Application.Interfaces.IoT;
using WineQuality.Application.Interfaces.Persistence;
using WineQuality.Application.Interfaces.Services;
using WineQuality.Application.Interfaces.Services.Identity;
using WineQuality.Infrastructure.Auth;
using WineQuality.Infrastructure.FileStorage;
using WineQuality.Infrastructure.Identity;
using WineQuality.Infrastructure.Identity.Services;
using WineQuality.Infrastructure.IoT;
using WineQuality.Infrastructure.Persistence;
using WineQuality.Infrastructure.Repositories;

namespace ChargingStation.Infrastructure;

public static class InfrastructureServicesRegistration
{

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // services.AddTransient<IUnitOfWork, UnitOfWork>();
        // services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        return services;
    }
}