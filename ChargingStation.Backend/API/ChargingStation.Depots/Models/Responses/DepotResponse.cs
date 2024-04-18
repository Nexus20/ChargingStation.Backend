using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Enums;

namespace ChargingStation.Depots.Models.Responses;

public class DepotResponse : BaseResponse, ITimeMarkable
{
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public required string Street { get; set; }
    public required string Building { get; set; }
    
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public DepotStatus Status { get; set; }
    
    public double? EnergyLimit { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}