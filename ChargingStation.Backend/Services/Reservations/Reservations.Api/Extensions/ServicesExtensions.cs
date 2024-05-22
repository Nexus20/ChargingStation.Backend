using System.Text;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Reservations.Api.EventConsumers;
using Reservations.Application.Extensions;

namespace Reservations.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddReservationServices(this IServiceCollection services, IConfiguration configuration)
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

        services.AddReservationApplicationServices(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<ReserveNowResponseConsumer>();
            busConfigurator.AddConsumer<CancelReservationResponseConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["MessageBrokerSettings:HostAddress"]);
                
                cfg.ReceiveEndpoint("reserve-now-response-queue-1_6", c => {
                    c.ConfigureConsumer<ReserveNowResponseConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("cancel-reservation-response-queue-1_6", c => {
                    c.ConfigureConsumer<CancelReservationResponseConsumer>(ctx);
                });
            });
        });

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"))
        );
        
        services.AddHangfireServer();

        return services;
    }
}