namespace ChargingStation.OcppTags.Models.Requests;

public class CreateOcppTagRequest
{
    public required string TagId { get; set; }
    
    public string? ParentTagId { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
}