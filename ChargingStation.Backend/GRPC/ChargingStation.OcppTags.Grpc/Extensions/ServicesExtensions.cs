using ChargingStation.OcppTags.Services;
using MassTransit;

namespace ChargingStation.OcppTags.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOcppTagGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        
        services.AddAutoMapper(typeof(IOcppTagService).Assembly);
        services.AddScoped<IOcppTagService, OcppTagService>();
        
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