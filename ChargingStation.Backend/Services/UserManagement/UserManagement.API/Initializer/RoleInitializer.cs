using ChargingStation.Common.Utility;
using ChargingStation.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.API.Initializer;

public class RoleInitializer : IRoleInitializer
{
    private readonly RoleManager<InfrastructureRole> _roleManager;

    public RoleInitializer(RoleManager<InfrastructureRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public void Initialize()
    {
        var roles = new List<InfrastructureRole>
        {
            new InfrastructureRole(CustomRoles.SuperAdministrator),
            new InfrastructureRole(CustomRoles.Administrator),
            new InfrastructureRole(CustomRoles.Employee),
            new InfrastructureRole(CustomRoles.Driver)
        };

        foreach (var role in roles)
        {
            if (_roleManager.RoleExistsAsync(role.Name!).Result)
                continue;

            _roleManager.CreateAsync(role);
        }
    }
}

