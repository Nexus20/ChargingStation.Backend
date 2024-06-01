using System.Text;
using ChargePoints.Api.EventConsumers;
using ChargePoints.Application.Extensions.DependencyInjection;
using ChargingStation.Common.Configurations;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ChargePoints.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargePointServices(this IServiceCollection services, IConfiguration configuration)
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

        services.AddChargePointsApplicationServices(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<BootNotificationConsumer>();
            busConfigurator.AddConsumer<DataTransferConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                var connectionString = configuration.GetSection(MessageBrokerConfiguration.SectionName).Get<MessageBrokerConfiguration>()!.GetConnectionString();
                cfg.Host(connectionString);
                
                cfg.ReceiveEndpoint("boot-notification-queue", c => {
                    c.ConfigureConsumer<BootNotificationConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("data-transfer-queue-1_6", c => {
                    c.ConfigureConsumer<DataTransferConsumer>(ctx);
                });
            });
        });

        return services;
    }
}
