using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class DepotEnergyConsumptionSettingsConfiguration : IEntityTypeConfiguration<DepotEnergyConsumptionSettings>
{
    public void Configure(EntityTypeBuilder<DepotEnergyConsumptionSettings> builder)
    {
        builder.HasMany(decs => decs.ChargePointsLimits)
            .WithOne(cpls => cpls.DepotEnergyConsumptionSettings)
            .HasForeignKey(cpls => cpls.DepotEnergyConsumptionSettingsId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ChargePointsLimits_DepotEnergyConsumptionSettings");
        
        builder.HasMany(decs => decs.Intervals)
            .WithOne(eis => eis.DepotEnergyConsumptionSettings)
            .HasForeignKey(eis => eis.DepotEnergyConsumptionSettingsId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Intervals_DepotEnergyConsumptionSettings");
    }
}