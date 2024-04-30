using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;
public class TimeZonesConfiguration : IEntityTypeConfiguration<Domain.Entities.TimeZone>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.TimeZone> builder)
    {
        builder.Property(e => e.BaseUtcOffset)
            .HasColumnType("time")
            .IsRequired()
            .HasConversion<TimeSpanToStringConverter>()
            .HasMaxLength(16);

        builder.Property(e => e.WindowsId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(e => e.IanaId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasMany<Depot>()
            .WithOne(d => d.TimeZone)
            .HasForeignKey(d => d.TimeZoneId);
    }
}
