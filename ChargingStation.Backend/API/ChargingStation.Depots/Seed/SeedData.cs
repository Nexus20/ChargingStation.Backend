using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace ChargingStation.Depots.Seed;

public class SeedData
{
    public static void SeedTimeZones(ApplicationDbContext context)
    {
        var json = File.ReadAllText("time_zones.json");
        var timeZones = JsonConvert.DeserializeObject<List<TimeZones>>(json);

        context.TimeZones.AddRange(timeZones!);
        context.SaveChanges();
    }
}

