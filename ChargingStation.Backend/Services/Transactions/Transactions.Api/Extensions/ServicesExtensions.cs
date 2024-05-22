using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Transactions.Api.EventConsumers;
using Transactions.Application.Extensions;

namespace Transactions.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddTransactionServices(this IServiceCollection services, IConfiguration configuration)
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

        services.AddTransactionApplicationServices(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<StartTransactionConsumer>();
            busConfigurator.AddConsumer<StopTransactionConsumer>();
            busConfigurator.AddConsumer<MeterValueConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("start-transaction-queue-1_6", c => {
                    c.ConfigureConsumer<StartTransactionConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("stop-transaction-queue-1_6", c => {
                    c.ConfigureConsumer<StopTransactionConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("meter-value-queue-1_6", c => {
                    c.ConfigureConsumer<MeterValueConsumer>(ctx);
                });
            });
        });

        return services;
    }
}
