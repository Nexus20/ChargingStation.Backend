using ChargingStation.Infrastructure.Identity;
using ChargingStation.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
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

