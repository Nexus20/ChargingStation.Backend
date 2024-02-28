﻿using ChargingStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStation.Infrastructure.Persistence.EntityConfigurations;

public class OcppTagConfiguration : IEntityTypeConfiguration<OcppTag>
{
    public void Configure(EntityTypeBuilder<OcppTag> builder)
    {
        builder.HasIndex(x => x.TagId).IsUnique();
    }
}