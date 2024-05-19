using ChargingStation.Common.Configurations;
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
                var connectionString = configuration.GetSection(MessageBrokerConfiguration.SectionName).Get<MessageBrokerConfiguration>()!.GetConnectionString();
                cfg.Host(connectionString);
                
                cfg.ReceiveEndpoint("authorize-queue", c => {
                    c.ConfigureConsumer<AuthorizeConsumer>(ctx);
                });
            });
        });
        
        return services;
    }
}