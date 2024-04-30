using ChargingStation.Common.Enums;
using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class Depot : Entity, ITimeMarkable
{
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public required string Street { get; set; }
    public required string Building { get; set; }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    
    public DepotStatus Status { get; set; }
    
    public List<ChargePoint> ChargePoints { get; set; }

    public required Guid TimeZoneId { get; set; }
    public TimeZones? TimeZone { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<DepotEnergyConsumptionSettings>? EnergyConsumptionSettings { get; set; }
}