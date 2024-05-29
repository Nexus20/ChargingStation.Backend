using System.Reflection;
using System.Text;
using ChargingStation.CacheManager.Extensions;
using ChargingStation.ChargingProfiles.EventConsumers;
using ChargingStation.ChargingProfiles.Services;
using ChargingStation.Common.Configurations;
using ChargingStation.InternalCommunication.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ChargingStation.ChargingProfiles.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddChargingProfilesServices(this IServiceCollection services,
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

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        services.AddDepotsHttpClient(configuration);
        
        services.AddCacheServices(configuration);
        services.AddChargePointsGrpcClient(configuration);
        services.AddConnectorsGrpcClient(configuration);
        services.AddTransactionsGrpcClient(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            
            busConfigurator.AddConsumer<SetChargingProfileResponseConsumer>();
            busConfigurator.AddConsumer<ClearChargingProfileResponseConsumer>();
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                var connectionString = configuration.GetSection(MessageBrokerConfiguration.SectionName).Get<MessageBrokerConfiguration>()!.GetConnectionString();
                cfg.Host(connectionString);
                
                cfg.ReceiveEndpoint("set-charging-profile-response-queue-1_6", c => {
                    c.ConfigureConsumer<SetChargingProfileResponseConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("clear-charging-profile-response-queue-1_6", c => {
                    c.ConfigureConsumer<ClearChargingProfileResponseConsumer>(ctx);
                });
            });
        });
        
        services.AddScoped<IChargingProfileService, ChargingProfileService>();
        
        return services;
    }
}