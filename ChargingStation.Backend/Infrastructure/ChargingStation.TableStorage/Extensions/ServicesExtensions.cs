﻿using Azure.Data.Tables;
using ChargingStation.TableStorage.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChargingStation.TableStorage.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddTableStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(ITableManager<>), typeof(AzureTableStorageManager<>));
        var connectionString = configuration.GetConnectionString("AzureTableStorage");

        services.AddScoped(_ => new TableServiceClient(connectionString));

        return services;
    }
}