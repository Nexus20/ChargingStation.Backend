using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class ChargingProfileConfiguration : IEntityTypeConfiguration<ChargingProfile>
{
    public void Configure(EntityTypeBuilder<ChargingProfile> builder)
    {
        builder.Property(cp => cp.ChargingProfileId).ValueGeneratedOnAdd();
        
        builder.HasMany(cp => cp.ChargingSchedulePeriods)
            .WithOne(csp => csp.ChargingProfile)
            .HasForeignKey(csp => csp.ChargingProfileId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ChargingSchedulePeriods_ChargingProfile");
        
        builder.HasMany(cp => cp.ConnectorChargingProfiles)
            .WithOne(ccp => ccp.ChargingProfile)
            .HasForeignKey(ccp => ccp.ChargingProfileId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ConnectorChargingProfiles_ChargingProfile");
    }
}