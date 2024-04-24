using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class ChargePointConfiguration : IEntityTypeConfiguration<ChargePoint>
{
    public void Configure(EntityTypeBuilder<ChargePoint> builder)
    {
        builder.HasMany(cp => cp.EnergyConsumptionSettings)
            .WithOne(ec => ec.ChargePoint)
            .HasForeignKey(ec => ec.ChargePointId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ChargePointEnergyConsumptionSettings_ChargePoint");
    }
}