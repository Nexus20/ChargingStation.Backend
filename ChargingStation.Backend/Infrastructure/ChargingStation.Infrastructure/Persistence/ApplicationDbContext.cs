using System.Reflection;
using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TimeZone = ChargingStation.Domain.Entities.TimeZone;

namespace ChargingStation.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<InfrastructureUser, InfrastructureRole, string, IdentityUserClaim<string>, InfrastructureUserRole,
    IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>> {

    public required DbSet<Depot> Depots { get; set; } 
    public required DbSet<ChargePoint> ChargePoints { get; set; }
    public required DbSet<OcppTag> OcppTags { get; set; }
    public required DbSet<OcppTransaction> Transactions { get; set; }
    public required DbSet<Connector> Connectors { get; set; }
    public required DbSet<Reservation> Reservations { get; set; }
    public required DbSet<TimeZone> TimeZones { get; set; }
    public required DbSet<ApplicationUser> Users { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        if (!Database.IsInMemory())
        {
            Database.Migrate();
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.NavigationBaseIncludeIgnored, CoreEventId.NavigationBaseIncluded));
        base.OnConfiguring(optionsBuilder);
    }

    public override int SaveChanges()
    {
        PreSaveChanges();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        PreSaveChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void PreSaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(x => x.Entity is ITimeMarkable && x.State is EntityState.Added or EntityState.Modified);
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((ITimeMarkable)entry.Entity).CreatedAt = DateTime.UtcNow;
            }
            ((ITimeMarkable)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(ApplicationDbContext))!);
    }
}