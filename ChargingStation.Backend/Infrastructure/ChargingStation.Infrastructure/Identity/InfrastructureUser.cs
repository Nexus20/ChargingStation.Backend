using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ChargingStation.Infrastructure.Identity;

public class InfrastructureUser : IdentityUser, ITimeMarkable
{
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public virtual List<InfrastructureUserRole>? UserRoles { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

