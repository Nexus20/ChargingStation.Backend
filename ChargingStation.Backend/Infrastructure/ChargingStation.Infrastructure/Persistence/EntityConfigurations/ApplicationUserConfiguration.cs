using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(x => x.OcppTag)
            .WithOne()
            .HasForeignKey<ApplicationUser>(x => x.OcppTagId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}