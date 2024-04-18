using MassTransit;
using SignalR.EventConsumers;

namespace SignalR.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddSignalRServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumer<SignalRResponseConsumer>();

                busConfigurator.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);

                    cfg.ReceiveEndpoint("signalR-queue", c => {
                        c.ConfigureConsumer<SignalRResponseConsumer>(ctx);
                    });
                });
            });

            return services;
        }
    }
}
