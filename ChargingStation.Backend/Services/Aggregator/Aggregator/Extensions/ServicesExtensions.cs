﻿using System.Reflection;
using System.Text;
using Aggregator.Services;
using Aggregator.Services.ChargePoints;
using Aggregator.Services.Connectors;
using ChargingStation.InternalCommunication.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Aggregator.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddAggregatorServices(this IServiceCollection services, IConfiguration configuration)
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
        
        services.AddScoped<IDepotsAggregatorService, DepotsAggregatorService>();

        services.AddDepotsHttpClient(configuration);
        services.AddEnergyConsumptionSettingsHttpClient(configuration);
        
        services.AddHttpClient<IChargePointsHttpService, ChargePointsHttpService>(c =>
        {
            c.BaseAddress = new Uri($"{configuration["ApiSettings:ChargePointsServiceAddress"]!}/api/chargepoint/");
        });
        
        services.AddHttpClient<IActiveChargePointsHttpService, ActiveChargePointsHttpService>(c =>
        {
            c.BaseAddress = new Uri($"{configuration["ApiSettings:WebsocketsServiceAddress"]!}/api/chargepoint/");
        });

        services.AddHttpClient<IConnectorsHttpService, ConnectorsHttpService>(c =>
        {
            c.BaseAddress = new Uri($"{configuration["ApiSettings:ConnectorsServiceAddress"]!}/api/connector/");
        });
        
        return services;
    }
}