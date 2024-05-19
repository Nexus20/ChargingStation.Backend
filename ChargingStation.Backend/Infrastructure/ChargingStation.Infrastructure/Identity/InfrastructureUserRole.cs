using ChargingStation.Common.Models.Abstract;
using Microsoft.AspNetCore.Identity;

namespace ChargingStation.Infrastructure.Identity;

public class InfrastructureUserRole : IdentityUserRole<string>, ITimeMarkable
{
    public InfrastructureUser User { get; set; }

    public InfrastructureRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

