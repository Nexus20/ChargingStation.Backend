using UserManagement.API.Initializers;

namespace UserManagement.API.Extensions;

public static class HostExtensions
{
    public static async Task<IHost> SeedDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        var adminInitializer = services.GetRequiredService<IAdminInitializer>();
        
        await adminInitializer.InitializeAsync();

        return host;
    }
}

