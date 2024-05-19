using ChargingStation.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ChargingStation.Infrastructure.Identity;

public class InfrastructureUser : IdentityUser
{
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public virtual List<InfrastructureUserRole>? UserRoles { get; set; }
}

