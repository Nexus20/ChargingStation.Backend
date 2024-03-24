using ChargingStation.Common.Models.Abstract;

namespace ChargingStation.Common.Models.OcppTags.Responses;

public class OcppTagResponse : ITimeMarkable
{
    public Guid Id { get; set; }
    
    public required string TagId { get; set; }
    
    public string? ParentTagId { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    public bool? Blocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}