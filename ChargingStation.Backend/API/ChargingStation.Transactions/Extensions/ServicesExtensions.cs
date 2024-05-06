using System.Reflection;
using ChargingStation.InternalCommunication.Extensions;
using ChargingStation.Mailing.Extensions;
using ChargingStation.Transactions.EventConsumers;
using ChargingStation.Transactions.Repositories.ConnectorMeterValues;
using ChargingStation.Transactions.Repositories.Transactions;
using ChargingStation.Transactions.Services.MeterValues;
using ChargingStation.Transactions.Services.Transactions;
using MassTransit;

namespace ChargingStation.Transactions.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddTransactionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IConnectorMeterValueRepository, ConnectorMeterValueRepository>();
        
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IMeterValueService, MeterValueService>();
        
        services.AddEnergyConsumptionSettingsHttpClient(configuration);
        services.AddReservationsHttpClient(configuration);
        
        services.AddMailingServices(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<StartTransactionConsumer>();
            busConfigurator.AddConsumer<StopTransactionConsumer>();
            busConfigurator.AddConsumer<MeterValueConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("start-transaction-queue-1_6", c => {
                    c.ConfigureConsumer<StartTransactionConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("stop-transaction-queue-1_6", c => {
                    c.ConfigureConsumer<StopTransactionConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("meter-value-queue-1_6", c => {
                    c.ConfigureConsumer<MeterValueConsumer>(ctx);
                });
            });
        });

        return services;
    }
}
