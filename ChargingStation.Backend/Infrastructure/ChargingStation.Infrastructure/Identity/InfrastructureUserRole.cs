using Microsoft.AspNetCore.Identity;

namespace ChargingStation.Infrastructure.Identity;

public class InfrastructureUserRole : IdentityUserRole<string>
{
    public virtual InfrastructureUser User { get; set; }

    public virtual InfrastructureRole Role { get; set; }
}

