using System.Reflection;
using ChargingStation.Transactions.EventConsumers;
using ChargingStation.Transactions.Repositories;
using ChargingStation.Transactions.Services.Connectors;
using ChargingStation.Transactions.Services.MeterValues;
using ChargingStation.Transactions.Services.OcppTags;
using ChargingStation.Transactions.Services.Transactions;
using MassTransit;

namespace ChargingStation.Transactions.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddTransactionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IMeterValueService, MeterValueService>();
        
        services.AddHttpClient<IOcppTagHttpService, OcppTagHttpService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:OcppTagServiceAddress"]!);
        });
        
        services.AddHttpClient<IConnectorHttpService, ConnectorHttpService>(c =>
        {
            c.BaseAddress = new Uri(configuration["ApiSettings:ConnectorServiceAddress"]!);
        });
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<StartTransactionConsumer>();
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("start-transaction-queue-1_6", c => {
                    c.ConfigureConsumer<StartTransactionConsumer>(ctx);
                });
            });
            
            busConfigurator.AddConsumer<StopTransactionConsumer>();
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("stop-transaction-queue-1_6", c => {
                    c.ConfigureConsumer<StopTransactionConsumer>(ctx);
                });
            });

            busConfigurator.AddConsumer<MeterValueConsumer>();
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("meter-value-queue-1_6", c => {
                    c.ConfigureConsumer<MeterValueConsumer>(ctx);
                });
            });
        });

        return services;
    }
}
