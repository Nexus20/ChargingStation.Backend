using UserManagement.API.Initializers;

namespace UserManagement.API.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddUserManagementServices(this IServiceCollection services)
    {
        //services.AddIdentity<InfrastructureUser, InfrastructureRole>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>()
        //    .AddDefaultTokenProviders();

        services.AddScoped<IAdminInitializer, AdminInitializer>();
        services.AddScoped<IRoleInitializer, RoleInitializer>();

        return services;
    }
}

