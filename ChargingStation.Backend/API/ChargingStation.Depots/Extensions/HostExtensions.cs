using Bogus;
using ChargingStation.Common.Enums;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Persistence;

namespace ChargingStation.Depots.Extensions;

public static class HostExtensions
{
    public static IHost SeedDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        if(context.Depots.Any())
            return host;

        Randomizer.Seed = new Random(123);
        
        var depots = new Faker<Depot>()
            .RuleFor(d => d.Name, f => f.Company.CompanyName())
            .RuleFor(d => d.Country, f => f.Address.Country())
            .RuleFor(d => d.City, f => f.Address.City())
            .RuleFor(d => d.Street, f => f.Address.StreetName())
            .RuleFor(d => d.Building, f => f.Address.BuildingNumber())
            .RuleFor(d => d.Status, f => DepotStatus.Available)
            .RuleFor(d => d.CreatedAt, f => DateTime.Now)
            .RuleFor(d => d.UpdatedAt, f => null)
            .Generate(10);
        
        context.Depots.AddRange(depots);
        context.SaveChanges();
        
        return host;
    }
}