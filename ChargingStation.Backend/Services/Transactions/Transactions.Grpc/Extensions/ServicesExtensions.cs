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
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
            });
        });

        return services;
    }
}