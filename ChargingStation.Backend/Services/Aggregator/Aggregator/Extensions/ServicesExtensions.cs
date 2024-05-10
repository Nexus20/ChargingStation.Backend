using System.Reflection;
using Aggregator.Services;
using Aggregator.Services.ChargePoints;
using Aggregator.Services.Connectors;
using ChargingStation.InternalCommunication.Extensions;

namespace Aggregator.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddAggregatorServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        services.AddScoped<IDepotsAggregatorService, DepotsAggregatorService>();

        services.AddDepotsHttpClient(configuration);
        services.AddEnergyConsumptionSettingsHttpClient(configuration);
        
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