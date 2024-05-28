using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class ApplicationUserDepotConfiguration : IEntityTypeConfiguration<ApplicationUserDepot>
{
    public void Configure(EntityTypeBuilder<ApplicationUserDepot> builder)
    {
        builder.HasOne(ud => ud.ApplicationUser)
            .WithMany(u => u.ApplicationUserDepots)
            .HasForeignKey(ud => ud.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ud => ud.Depot)
            .WithMany(d => d.ApplicationUserDepots)
            .HasForeignKey(ud => ud.DepotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
