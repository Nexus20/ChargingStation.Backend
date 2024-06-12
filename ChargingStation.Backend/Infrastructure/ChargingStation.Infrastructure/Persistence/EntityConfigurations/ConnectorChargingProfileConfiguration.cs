using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class ConnectorChargingProfileConfiguration : IEntityTypeConfiguration<ConnectorChargingProfile>
{
    public void Configure(EntityTypeBuilder<ConnectorChargingProfile> builder)
    {
        builder.HasIndex(x => new { x.ConnectorId, x.ChargingProfileId }).IsUnique();
    }
}