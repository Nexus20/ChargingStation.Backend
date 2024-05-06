using ChargingStation.InternalCommunication.Extensions;
using ChargingStation.Mailing.Extensions;
using ChargingStation.Transactions.Repositories.Transactions;
using ChargingStation.Transactions.Services.Transactions;
using MassTransit;

namespace ChargingStation.Transactions.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddTransactionGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        
        services.AddAutoMapper(typeof(ITransactionService).Assembly);
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        
        services.AddScoped<ITransactionService, TransactionService>();
        
        services.AddEnergyConsumptionSettingsHttpClient(configuration);
        services.AddChargePointsGrpcClient(configuration);
        services.AddOcppTagsGrpcClient(configuration);
        services.AddConnectorsGrpcClient(configuration);
        services.AddReservationsGrpcClient(configuration);
        
        services.AddMailingServices(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
            });
        });

        return services;
    }
}