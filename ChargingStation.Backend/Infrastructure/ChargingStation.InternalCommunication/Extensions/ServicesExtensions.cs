using ChargePoints.Grpc.Protos;
using ChargingStation.InternalCommunication.GrpcClients;
using ChargingStation.InternalCommunication.Services.Depots;
using ChargingStation.InternalCommunication.Services.EnergyConsumption;
using Connectors.Grpc.Protos;
using Depots.Grpc.Protos;
using EnergyConsumption.Grpc.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OcppTags.Grpc.Protos;
using Reservations.Grpc.Protos;
using Transactions.Grpc.Protos;

namespace ChargingStation.InternalCommunication.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddEnergyConsumptionSettingsGrpcClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpcClient<EnergyConsumptionSettingsGrpc.EnergyConsumptionSettingsGrpcClient>
            (o => o.Address = new Uri(configuration["GrpcSettings:EnergyConsumptionSettingsServiceAddress"]!));
        services.AddScoped<EnergyConsumptionSettingsGrpcClientService>();
        
        return services;
    }
    
    public static IServiceCollection AddTransactionsGrpcClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpcClient<TransactionsGrpc.TransactionsGrpcClient>
            (o => o.Address = new Uri(configuration["GrpcSettings:TransactionServiceAddress"]!));
        services.AddScoped<TransactionGrpcClientService>();
        
        return services;
    }
    
    public static IServiceCollection AddReservationsGrpcClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpcClient<ReservationsGrpc.ReservationsGrpcClient>
            (o => o.Address = new Uri(configuration["GrpcSettings:ReservationServiceAddress"]!));
        services.AddScoped<ReservationGrpcClientService>();
        
        return services;
    }
    
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
    
    public static IServiceCollection AddDepotsGrpcClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpcClient<DepotsGrpc.DepotsGrpcClient>
            (o => o.Address = new Uri(configuration["GrpcSettings:DepotServiceAddress"]!));
        services.AddScoped<DepotGrpcClientService>();
        
        return services;
    }
    
    public static IServiceCollection AddDepotsHttpClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient<IDepotHttpService, DepotHttpService>((sp, c) =>
        {
            var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var authorizationHeader = contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();
            
            c.BaseAddress = new Uri(configuration["ApiSettings:DepotServiceAddress"]!);
            c.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
        });

        return services;
    }

    public static IServiceCollection AddEnergyConsumptionSettingsHttpClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient<IEnergyConsumptionHttpService, EnergyConsumptionHttpService>((sp, c) =>
        {
            var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var authorizationHeader = contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();
            
            c.BaseAddress = new Uri(configuration["ApiSettings:EnergyConsumptionSettingsServiceAddress"]!);
            c.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
        });

        return services;
    }
}