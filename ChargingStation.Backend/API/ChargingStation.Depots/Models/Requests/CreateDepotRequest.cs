namespace ChargingStation.Depots.Models.Requests;

public class CreateDepotRequest
{
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public required string Street { get; set; }
    public required string Building { get; set; }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public double? EnergyLimit { get; set; }
    
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
}