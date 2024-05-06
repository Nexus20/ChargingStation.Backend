using ChargePoints.Grpc;
using ChargingStation.InternalCommunication.GrpcClients;
using ChargingStation.InternalCommunication.Services.ChargePoints;
using ChargingStation.InternalCommunication.Services.Connectors;
using ChargingStation.InternalCommunication.Services.Depots;
using ChargingStation.InternalCommunication.Services.EnergyConsumption;
using ChargingStation.InternalCommunication.Services.OcppTags;
using ChargingStation.InternalCommunication.Services.Reservations;
using ChargingStation.InternalCommunication.Services.Transactions;
using Connectors.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OcppTags.Grpc;

namespace ChargingStation.InternalCommunication.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOcppTagsGrpcClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpcClient<OcppTagsGrpc.OcppTagsGrpcClient>
            (o => o.Address = new Uri(configuration["GrpcSettings:OcppTagServiceAddress"]!));
        services.AddScoped<OcppTagGrpcClientService>();
        
        return services;
    }
    
    public static IServiceCollection AddConnectorsGrpcClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpcClient<ConnectorsGrpc.ConnectorsGrpcClient>
            (o => o.Address = new Uri(configuration["GrpcSettings:ConnectorServiceAddress"]!));
        services.AddScoped<ConnectorGrpcClientService>();
        
        return services;
    }
    
    public static IServiceCollection AddChargePointsGrpcClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpcClient<ChargePointsGrpc.ChargePointsGrpcClient>
            (o => o.Address = new Uri(configuration["GrpcSettings:ChargePointServiceAddress"]!));
        services.AddScoped<ChargePointGrpcClientService>();
        
        return services;
    }
    
    public static IServiceCollection AddDepotsHttpClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IDepotHttpService, DepotHttpService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:DepotServiceAddress"]!);
        });

        return services;
    }
    
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
    
    public static IServiceCollection AddTransactionsHttpClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<ITransactionHttpService, TransactionHttpService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:TransactionServiceAddress"]!);
        });

        return services;
    }
    
    public static IServiceCollection AddReservationsHttpClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IReservationHttpService, ReservationHttpService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:ReservationServiceAddress"]!);
        });

        return services;
    }
    
    public static IServiceCollection AddEnergyConsumptionSettingsHttpClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IEnergyConsumptionHttpService, EnergyConsumptionHttpService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:EnergyConsumptionServiceAddress"]!);
        });

        return services;
    }
}