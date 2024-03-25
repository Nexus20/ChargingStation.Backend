﻿using ChargingStation.Domain.Entities;
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