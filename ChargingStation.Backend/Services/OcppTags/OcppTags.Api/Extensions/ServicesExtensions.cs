using MassTransit;
using OcppTags.Api.EventConsumers;
using OcppTags.Application.Extensions;

namespace OcppTags.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOcppTagServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcppTagApplicationServices();
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<AuthorizeConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("authorize-queue", c => {
                    c.ConfigureConsumer<AuthorizeConsumer>(ctx);
                });
            });
        });
        
        return services;
    }
}