using UserManagement.API.Initializers;
using UserManagement.API.Services;
using System.Reflection;
using UserManagement.API.Utility;

namespace UserManagement.API.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddUserManagementServices(this IServiceCollection services, IConfiguration configuration)
    {
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

