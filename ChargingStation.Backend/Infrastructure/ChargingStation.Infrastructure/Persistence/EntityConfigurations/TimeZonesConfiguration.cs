using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;
public class TimeZonesConfiguration : IEntityTypeConfiguration<Domain.Entities.TimeZone>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.TimeZone> builder)
    {
        builder.Property(e => e.BaseUtcOffset)
            .IsRequired()
            .HasConversion<TimeSpanToStringConverter>()
            .HasMaxLength(16);

        builder.Property(e => e.WindowsId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(e => e.IanaId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(e => e.DisplayName)
            .HasMaxLength(128)
            .IsRequired();
    }
}
