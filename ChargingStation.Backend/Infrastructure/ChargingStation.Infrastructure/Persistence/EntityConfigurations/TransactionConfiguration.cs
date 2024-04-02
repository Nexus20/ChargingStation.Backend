using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class TransactionConfiguration : IEntityTypeConfiguration<OcppTransaction>
{
    public void Configure(EntityTypeBuilder<OcppTransaction> builder)
    {
        builder.Property(t => t.TransactionId).ValueGeneratedOnAdd();
        
        builder.Property(e => e.StartResult).HasMaxLength(100);

        builder.Property(e => e.StartTagId).HasMaxLength(50);

        builder.Property(e => e.StopReason).HasMaxLength(100);

        builder.Property(e => e.StopTagId).HasMaxLength(50);
        
        builder.HasMany(t => t.ConnectorMeterValues)
            .WithOne(cm => cm.Transaction)
            .HasForeignKey(cm => cm.TransactionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ConnectorMeterValues_Transaction");
    }
}