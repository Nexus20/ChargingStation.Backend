using Azure.Data.Tables;
using ChargingStation.Heartbeats.EventConsumers;
using ChargingStation.Heartbeats.Services.Heartbeats;
using ChargingStation.Infrastructure.AzureTableStorage;
using ChargingStation.InternalCommunication.Extensions;
using MassTransit;

namespace ChargingStation.Heartbeats.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddHeartbeatServices(this IServiceCollection services,
        IConfiguration configuration)
    {
            services.AddChargePointsHttpClient(configuration);
            services.AddScoped<IHeartbeatService, HeartbeatService>();

            services.AddScoped(typeof(ITableManager<>), typeof(AzureTableStorageManager<>));
            var connectionString = configuration.GetConnectionString("AzureTableStorage:AzureTableStorageConnection");
            services.AddScoped(_ => new TableServiceClient(connectionString));


            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumer<HeartbeatConsumer>();

                busConfigurator.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);

                    cfg.ReceiveEndpoint("heartbeat-queue", c => {
                        c.ConfigureConsumer<HeartbeatConsumer>(ctx);
                    });
                });
            });


            return services;
        }
}