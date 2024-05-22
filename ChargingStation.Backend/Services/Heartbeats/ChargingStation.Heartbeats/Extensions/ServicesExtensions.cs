using System.Text;
using ChargingStation.Heartbeats.EventConsumers;
using ChargingStation.Heartbeats.Services.Heartbeats;
using ChargingStation.Infrastructure.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ChargingStation.Heartbeats.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddHeartbeatServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
                };
            });

        services.AddTableStorageServices(configuration);
        services.AddScoped<IHeartbeatService, HeartbeatService>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<HeartbeatConsumer>();

            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);

                cfg.ReceiveEndpoint("heartbeat-queue", c => { c.ConfigureConsumer<HeartbeatConsumer>(ctx); });
            });
        });
        
        return services;
    }
}