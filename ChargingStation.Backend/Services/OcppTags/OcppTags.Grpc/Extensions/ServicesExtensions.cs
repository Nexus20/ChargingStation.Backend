using MassTransit;
using OcppTags.Application.Extensions;

namespace OcppTags.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOcppTagGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        
        services.AddOcppTagApplicationServices();
        
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