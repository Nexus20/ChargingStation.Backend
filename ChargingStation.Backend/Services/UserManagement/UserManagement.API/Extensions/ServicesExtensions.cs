using System.Text;
using ChargingStation.Infrastructure.Identity;
using ChargingStation.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using UserManagement.API.Initializers;
using UserManagement.API.Persistence;
using UserManagement.API.Services;
using System.Reflection;

namespace UserManagement.API.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddUserManagementServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<InfrastructureUser, InfrastructureRole>()
            .AddUserStore<UserStore<InfrastructureUser, InfrastructureRole, ApplicationDbContext, string, IdentityUserClaim<string>, InfrastructureUserRole,
                IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
            .AddRoleStore<RoleStore<InfrastructureRole, ApplicationDbContext, string, InfrastructureUserRole, IdentityRoleClaim<string>>>()
            .AddSignInManager<SignInManager<InfrastructureUser>>()
            .AddRoleManager<RoleManager<InfrastructureRole>>()
            .AddUserManager<UserManager<InfrastructureUser>>();

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

        services.AddScoped<JwtHandler>(provider =>
            new JwtHandler(
                configuration["Jwt:SecretKey"]!,
                configuration["Jwt:Issuer"]!,
                configuration["Jwt:Audience"]!));

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IAdminInitializer, AdminInitializer>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}

