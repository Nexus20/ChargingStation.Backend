using ChargingStation.Infrastructure.Persistence;
using Newtonsoft.Json;
using TimeZone = ChargingStation.Domain.Entities.TimeZone;

namespace ChargingStation.Depots.Seed;

public class SeedData
{
    public static void SeedTimeZones(ApplicationDbContext context)
    {
        var json = File.ReadAllText("Seed/time_zones.json");
        var timeZones = JsonConvert.DeserializeObject<List<TimeZone>>(json);

        context.TimeZones.AddRange(timeZones!);
        context.SaveChanges();
    }
}

