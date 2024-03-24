using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.Property(r => r.ReservationId).ValueGeneratedOnAdd();

        builder.HasOne(r => r.ChargePoint)
            .WithMany(cp => cp.Reservations)
            .HasForeignKey(r => r.ChargePointId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Reservations_ChargePoint");
        
        builder.HasOne(r => r.Connector)
            .WithOne(c => c.Reservation)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(r => r.Transaction)
            .WithOne(t => t.Reservation)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);
        
        builder.HasIndex(x => x.ReservationRequestId).IsUnique();
    }
}

public class ConnectorConfiguration : IEntityTypeConfiguration<Connector>
{
    public void Configure(EntityTypeBuilder<Connector> builder)
    {
        builder.Property(c => c.ConnectorId).ValueGeneratedOnAdd();
        
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
    }
}