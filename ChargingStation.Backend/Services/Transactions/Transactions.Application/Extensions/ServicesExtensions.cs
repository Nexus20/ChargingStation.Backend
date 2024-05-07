using System.Reflection;
using ChargingStation.InternalCommunication.Extensions;
using ChargingStation.Mailing.Extensions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Transactions.Application.Repositories.ConnectorMeterValues;
using Transactions.Application.Repositories.Transactions;
using Transactions.Application.Services.MeterValues;
using Transactions.Application.Services.Transactions;

namespace Transactions.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddTransactionApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IConnectorMeterValueRepository, ConnectorMeterValueRepository>();
        
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IMeterValueService, MeterValueService>();
        
        services.AddEnergyConsumptionSettingsHttpClient(configuration);
        services.AddChargePointsGrpcClient(configuration);
        services.AddOcppTagsGrpcClient(configuration);
        services.AddConnectorsGrpcClient(configuration);
        services.AddReservationsGrpcClient(configuration);
        
        services.AddMailingServices(configuration);

        return services;
    }
}
