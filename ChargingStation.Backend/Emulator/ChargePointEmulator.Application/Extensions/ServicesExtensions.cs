using System.Reflection;
using ChargePointEmulator.Application.Services;
using ChargingStation.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChargePointEmulator.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargePointServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IChargePointService, ChargePointService>();
        services.AddScoped<IOcppTagService, OcppTagService>();
        services.AddInfrastructureServices(configuration);
        services.AddSingleton<ChargingStationSimulatorManager>();

        services.AddHttpClient<IReservationHttpService, ReservationHttpService>(client =>
        {
            client.BaseAddress = new Uri($"{configuration["ApiSettings:ReservationServiceAddress"]!}/api/Reservation");
        });
        
        return services;
    }
}
