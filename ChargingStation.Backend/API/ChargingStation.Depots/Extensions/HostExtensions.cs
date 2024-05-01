using Bogus;
using ChargingStation.Common.Enums;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Persistence;
using Newtonsoft.Json;
using TimeZone = ChargingStation.Domain.Entities.TimeZone;

namespace ChargingStation.Depots.Extensions;

public static class HostExtensions
{
    public static IHost SeedDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        if(context.Depots.Any() && context.TimeZones.Any())
            return host;

        SeedTimeZones(context);

        Randomizer.Seed = new Random(123);

        var timeZones = context.TimeZones.ToList();
        var depots = new Faker<Depot>()
            .RuleFor(d => d.Name, f => f.Company.CompanyName())
            .RuleFor(d => d.Country, f => f.Address.Country())
            .RuleFor(d => d.City, f => f.Address.City())
            .RuleFor(d => d.Street, f => f.Address.StreetName())
            .RuleFor(d => d.Building, f => f.Address.BuildingNumber())
            .RuleFor(d => d.Status, f => DepotStatus.Available)
            .RuleFor(d => d.CreatedAt, f => DateTime.Now)
            .RuleFor(d => d.UpdatedAt, f => null)
            .RuleFor(d => d.TimeZoneId, f => f.PickRandom(timeZones).Id)
            .Generate(10);
        
        context.Depots.AddRange(depots);
        context.SaveChanges();
        
        return host;
    }

    private static void SeedTimeZones(ApplicationDbContext context)
    {
        var json = File.ReadAllText("time_zones.json");
        var timeZones = JsonConvert.DeserializeObject<List<TimeZone>>(json);

        context.TimeZones.AddRange(timeZones!);
        context.SaveChanges();
    }
}