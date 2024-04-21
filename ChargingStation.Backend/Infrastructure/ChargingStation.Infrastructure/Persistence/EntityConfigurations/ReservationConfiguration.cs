using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.Property(t => t.ReservationId).UseIdentityColumn();
        builder.Property(t => t.ReservationId).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        
        builder.HasOne(r => r.ChargePoint)
            .WithMany(cp => cp.Reservations)
            .HasForeignKey(r => r.ChargePointId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Reservations_ChargePoint");
        
        builder.HasOne(r => r.Connector)
            .WithMany(c => c.Reservations)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(r => r.Transaction)
            .WithOne(t => t.Reservation)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);
        
        builder.HasIndex(x => x.ReservationRequestId).IsUnique();
    }
}