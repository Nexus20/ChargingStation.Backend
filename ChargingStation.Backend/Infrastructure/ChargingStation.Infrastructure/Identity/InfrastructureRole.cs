using ChargingStation.Common.Models.Abstract;
using Microsoft.AspNetCore.Identity;

namespace ChargingStation.Infrastructure.Identity;

public class InfrastructureRole : IdentityRole, ITimeMarkable
{
    public InfrastructureRole() { }
    public InfrastructureRole(string role) : base(role) { }

    public virtual List<InfrastructureUserRole> UserRoles { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

