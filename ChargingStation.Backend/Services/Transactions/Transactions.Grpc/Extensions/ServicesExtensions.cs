using ChargingStation.Common.Configurations;
using MassTransit;
using Transactions.Application.Extensions;

namespace Transactions.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddTransactionGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        services.AddTransactionApplicationServices(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                var connectionString = configuration.GetSection(MessageBrokerConfiguration.SectionName).Get<MessageBrokerConfiguration>()!.GetConnectionString();
                cfg.Host(connectionString);
            });
        });

        return services;
    }
}