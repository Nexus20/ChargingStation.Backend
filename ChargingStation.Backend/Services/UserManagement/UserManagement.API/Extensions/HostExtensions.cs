using UserManagement.API.Initializers;

namespace UserManagement.API.Extensions;

public static class HostExtensions
{
    public static IHost SeedDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var admin = services.GetRequiredService<IAdminInitializer>();
        var role = services.GetRequiredService<IRoleInitializer>();

        admin.Initialize();
        role.Initialize();

        return host;
    }
}

