using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class OcppTag : Entity, ITimeMarkable
{
    public required string TagId { get; set; }
    
    public string? ParentTagId { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    public bool? Blocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}