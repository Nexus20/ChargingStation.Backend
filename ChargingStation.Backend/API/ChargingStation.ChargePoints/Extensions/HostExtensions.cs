using Bogus;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Persistence;

namespace ChargingStation.ChargePoints.Extensions;

public static class HostExtensions
{
    public static IHost SeedDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        if(context.ChargePoints.Any())
            return host;

        var depotsIds = context.Depots.Select(x => x.Id).ToList();
        
        Randomizer.Seed = new Random(123);
        
        var chargePoints = new Faker<ChargePoint>()
            .RuleFor(cp => cp.Name, f => $"CP {f.IndexVariable}")
            .RuleFor(d => d.DepotId, f => f.PickRandom(depotsIds))
            .RuleFor(d => d.CreatedAt, f => DateTime.Now)
            .RuleFor(d => d.UpdatedAt, f => null)
            .Generate(10);
        
        context.ChargePoints.AddRange(chargePoints);
        context.SaveChanges();
        
        return host;
    }
}