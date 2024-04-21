using ChargingStation.Aggregator.Services;
using ChargingStation.Aggregator.Services.ChargePoints;
using ChargingStation.Aggregator.Services.Connectors;
using ChargingStation.Aggregator.Services.Depots;

namespace ChargingStation.Aggregator.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddAggregatorServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDepotsAggregatorService, DepotsAggregatorService>();
        
        services.AddHttpClient<IDepotsHttpService, DepotsHttpService>(c =>
        {
            c.BaseAddress = new Uri($"{configuration["ApiSettings:DepotsServiceAddress"]!}/api/depot/");
        });
        
        services.AddHttpClient<IChargePointsHttpService, ChargePointsHttpService>(c =>
        {
            c.BaseAddress = new Uri($"{configuration["ApiSettings:ChargePointsServiceAddress"]!}/api/chargepoint/");
        });
        
        services.AddHttpClient<IActiveChargePointsHttpService, ActiveChargePointsHttpService>(c =>
        {
            c.BaseAddress = new Uri($"{configuration["ApiSettings:WebsocketsServiceAddress"]!}/api/chargepoint/");
        });

        services.AddHttpClient<IConnectorsHttpService, ConnectorsHttpService>(c =>
        {
            c.BaseAddress = new Uri($"{configuration["ApiSettings:ConnectorsServiceAddress"]!}/api/connector/");
        });
        
        return services;
    }
}