namespace ChargingStation.Depots.Models.Requests;

public class UpdateDepotRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Building { get; set; }
    
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public double? EnergyLimit { get; set; }
    
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
}