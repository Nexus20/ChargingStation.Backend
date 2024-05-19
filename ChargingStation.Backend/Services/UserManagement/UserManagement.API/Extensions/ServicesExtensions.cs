using UserManagement.API.Initializers;

namespace UserManagement.API.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddUserManagementServices(this IServiceCollection services)
    {
        services.AddScoped<IAdminInitializer, AdminInitializer>();

        return services;
    }
}

