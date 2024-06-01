using ChargingStation.Common.Rbac;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Identity;
using ChargingStation.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.API.Initializers;

public class AdminInitializer : IAdminInitializer
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<InfrastructureUser> _userManager;
    private readonly RoleManager<InfrastructureRole> _roleManager;
    public AdminInitializer(ApplicationDbContext db, UserManager<InfrastructureUser> userManager,
        RoleManager<InfrastructureRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeAsync()
    {
        if (!await _roleManager.RoleExistsAsync(CustomRoles.SuperAdministrator))
        {
            await InitializeRolesAsync();
        }
        
        if(await _userManager.FindByEmailAsync("admin@gmail.com") is not null)
            return;

        await using var transaction = await _db.Database.BeginTransactionAsync();
        
        try
        {
            await InitializeAdminAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new Exception(e.Message);
        }
    }
    
    private async Task InitializeAdminAsync()
    {
        var id = Guid.NewGuid();
        await _db.Users.AddAsync(new ApplicationUser()
        {
            Id = id,
            FirstName = "Admin",
            LastName = "Global",
            Email = "admin@gmail.com",
            Phone = "1234567890",

        });

        await _userManager.CreateAsync(new InfrastructureUser
        {
            Id = id.ToString(),
            ApplicationUserId = id,
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com",
            PhoneNumber = "1234567890",
        }, "Admin123*");

        var user = await _userManager.FindByIdAsync(id.ToString());
        await _userManager.AddToRolesAsync(user!, new List<string>()
        {
            CustomRoles.SuperAdministrator,
            CustomRoles.Administrator
        });
    }
    
    private async Task InitializeRolesAsync()
    {
        var roles = new List<InfrastructureRole>
        {
            new(CustomRoles.SuperAdministrator),
            new(CustomRoles.Administrator),
            new(CustomRoles.Employee),
            new(CustomRoles.Driver)
        };

        foreach (var role in roles)
        {
            if (await _roleManager.RoleExistsAsync(role.Name!))
                continue;

            await _roleManager.CreateAsync(role);
        }
    }
}

