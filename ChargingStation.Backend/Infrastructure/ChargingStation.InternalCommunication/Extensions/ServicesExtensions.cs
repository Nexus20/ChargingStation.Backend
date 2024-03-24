using ChargingStation.InternalCommunication.Services.ChargePoints;
using ChargingStation.InternalCommunication.Services.Connectors;
using ChargingStation.InternalCommunication.Services.OcppTags;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChargingStation.InternalCommunication.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOcppTagsHttpClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IOcppTagHttpService, OcppTagHttpService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:OcppTagServiceAddress"]!);
        });

        return services;
    }

    public static IServiceCollection AddConnectorsHttpClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IConnectorHttpService, ConnectorHttpService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:ConnectorServiceAddress"]!);
        });

        return services;
    }

    public static IServiceCollection AddChargePointsHttpClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IChargePointHttpService, ChargePointHttpService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:ChargePointServiceAddress"]!);
        });

        return services;
    }
}