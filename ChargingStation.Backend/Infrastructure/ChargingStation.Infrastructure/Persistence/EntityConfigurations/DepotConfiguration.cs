using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class DepotConfiguration : IEntityTypeConfiguration<Depot>
{
    public void Configure(EntityTypeBuilder<Depot> builder)
    {
        builder.HasMany(d => d.EnergyConsumptionSettings)
            .WithOne(decs => decs.Depot)
            .HasForeignKey(decs => decs.DepotId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DepotEnergyConsumptionSettings_Depot");
        
        builder.HasOne(d => d.TimeZone)
            .WithMany(tz => tz.Depots)
            .HasForeignKey(d => d.TimeZoneId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Depot_TimeZone");
    }
}