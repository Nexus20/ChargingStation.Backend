using System.Reflection;
using System.Text;
using Aggregator.Services;
using Aggregator.Services.Connectors;
using Aggregator.Services.Depots;
using Aggregator.Services.HttpServices.ChargePoints;
using Aggregator.Services.HttpServices.Connectors;
using Aggregator.Services.HttpServices.EnergyConsumption;
using Aggregator.Services.HttpServices.EnergyConsumptionSettings;
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
        services.AddScoped<IConnectorsAggregatorService, ConnectorsAggregatorService>();

        services.AddHttpContextAccessor();
        services.AddDepotsHttpClient(configuration);
        
        services.AddHttpClient<IEnergyConsumptionSettingsHttpService, EnergyConsumptionSettingsHttpService>((sp, c) =>
        {
            var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var authorizationHeader = contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();
            
            c.BaseAddress = new Uri($"{configuration["ApiSettings:EnergyConsumptionSettingsServiceAddress"]!}/api/EnergyConsumptionSettings/");
            c.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
        });
        
        services.AddHttpClient<IChargePointsHttpService, ChargePointsHttpService>((sp, c) =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var accessToken = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
            
            c.BaseAddress = new Uri($"{configuration["ApiSettings:ChargePointsServiceAddress"]!}/api/chargepoint/");
            c.DefaultRequestHeaders.Add("Authorization", accessToken);
        });
        
        services.AddHttpClient<IActiveChargePointsHttpService, ActiveChargePointsHttpService>((sp, c) =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var accessToken = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
            
            c.BaseAddress = new Uri($"{configuration["ApiSettings:WebsocketsServiceAddress"]!}/api/chargepoint/");
            c.DefaultRequestHeaders.Add("Authorization", accessToken);
        });

        services.AddHttpClient<IConnectorsHttpService, ConnectorsHttpService>((sp, c) =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var accessToken = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
            
            c.BaseAddress = new Uri($"{configuration["ApiSettings:ConnectorsServiceAddress"]!}/api/connector/");
            c.DefaultRequestHeaders.Add("Authorization", accessToken);
        });

        services.AddHttpClient<IEnergyConsumptionHttpService, EnergyConsumptionHttpService>((sp, c) =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var accessToken = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

            c.BaseAddress = new Uri($"{configuration["ApiSettings:EnergyConsumptionServiceAddress"]!}/api/energyconsumption/");
            c.DefaultRequestHeaders.Add("Authorization", accessToken);
        });
        
        return services;
    }
}