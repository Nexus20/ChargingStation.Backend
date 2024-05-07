using System.Reflection;
using MassTransit;
using OcppTags.Api.EventConsumers;
using OcppTags.Application.Services;

namespace OcppTags.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOcppTagServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IOcppTagService, OcppTagService>();
        
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