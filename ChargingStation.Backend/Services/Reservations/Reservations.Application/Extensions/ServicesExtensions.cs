using System.Reflection;
using ChargingStation.InternalCommunication.Extensions;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reservations.Application.Services.Reservations;

namespace Reservations.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddReservationApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddChargePointsGrpcClient(configuration);
        services.AddOcppTagsGrpcClient(configuration);
        services.AddConnectorsGrpcClient(configuration);
        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IReservationService, ReservationService>();

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"))
        );
        
        services.AddHangfireServer();

        return services;
    }
}