using ChargingStation.Infrastructure.Identity;
using ChargingStation.Infrastructure.Persistence;
using ChargingStation.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChargingStation.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddIdentity<InfrastructureUser, InfrastructureRole>()
            .AddUserStore<UserStore<InfrastructureUser, InfrastructureRole, ApplicationDbContext, string, IdentityUserClaim<string>, InfrastructureUserRole,
                IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
            .AddRoleStore<RoleStore<InfrastructureRole, ApplicationDbContext, string, InfrastructureUserRole, IdentityRoleClaim<string>>>()
            .AddSignInManager<SignInManager<InfrastructureUser>>()
            .AddRoleManager<RoleManager<InfrastructureRole>>()
            .AddUserManager<UserManager<InfrastructureUser>>();

        services.AddScoped<RoleManager<InfrastructureRole>>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        return services;
    }
}