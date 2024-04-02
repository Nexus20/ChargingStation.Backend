using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class ConnectorConfiguration : IEntityTypeConfiguration<Connector>
{
    public void Configure(EntityTypeBuilder<Connector> builder)
    {
        builder.HasIndex(x => new { x.ChargePointId, x.ConnectorId }).IsUnique();
        
        builder.HasOne(d => d.ChargePoint)
            .WithMany(p => p.Connectors)
            .HasForeignKey(d => d.ChargePointId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Connectors_ChargePoint");
        
        builder.HasMany(c => c.ConnectorStatuses)
            .WithOne(cs => cs.Connector)
            .HasForeignKey(cs => cs.ConnectorId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ConnectorStatuses_Connector");
        
        builder.HasMany(c => c.ConnectorMeterValues)
            .WithOne(cm => cm.Connector)
            .HasForeignKey(cm => cm.ConnectorId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ConnectorMeterValues_Connector");
        
        builder.HasMany(c => c.ConnectorChargingProfiles)
            .WithOne(cp => cp.Connector)
            .HasForeignKey(cp => cp.ConnectorId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ConnectorChargingProfiles_Connector");
    }
}