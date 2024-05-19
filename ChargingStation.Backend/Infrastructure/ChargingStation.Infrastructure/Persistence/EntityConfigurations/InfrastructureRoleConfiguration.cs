using ChargingStation.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class InfrastructureRoleConfiguration : IEntityTypeConfiguration<InfrastructureRole>
{
    public void Configure(EntityTypeBuilder<InfrastructureRole> builder)
    {
        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();
    }
}

