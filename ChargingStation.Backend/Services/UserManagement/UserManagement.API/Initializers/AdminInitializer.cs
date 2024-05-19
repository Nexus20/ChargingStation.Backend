using ChargingStation.Common.Utility;
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

    public async Task Initialize()
    {
        if (!_roleManager.RoleExistsAsync(CustomRoles.SuperAdministrator).GetAwaiter().GetResult())
        {
            await _roleManager.CreateAsync(new InfrastructureRole(CustomRoles.SuperAdministrator));
            await _roleManager.CreateAsync(new InfrastructureRole(CustomRoles.Administrator));
        }
        else
        {
            return;
        }

        var id = Guid.NewGuid();
        await _db.Users.AddAsync(new ApplicationUser()
        {
            Id = id,
            FirstName = "Admin",
            LastName = "Global"
            
        });

        await _userManager.CreateAsync(new InfrastructureUser
        {
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

        await _db.SaveChangesAsync();
    }
}

