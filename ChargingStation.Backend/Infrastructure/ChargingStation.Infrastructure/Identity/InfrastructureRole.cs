using Microsoft.AspNetCore.Identity;

namespace ChargingStation.Infrastructure.Identity;

public class InfrastructureRole : IdentityRole
{
    public InfrastructureRole() { }
    public InfrastructureRole(string role) : base(role) { }

    public virtual List<InfrastructureUserRole> UserRoles { get; set; }
}

