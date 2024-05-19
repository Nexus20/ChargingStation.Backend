using ChargingStation.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class InfrastructureUserConfiguration : IEntityTypeConfiguration<InfrastructureUser>
{
    public void Configure(EntityTypeBuilder<InfrastructureUser> builder)
    {
        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}

