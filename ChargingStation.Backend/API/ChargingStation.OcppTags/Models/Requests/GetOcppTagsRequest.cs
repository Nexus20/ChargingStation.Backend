using ChargingStation.Common.Models.General.Requests;

namespace ChargingStation.OcppTags.Models.Requests;

public class GetOcppTagsRequest : BaseCollectionRequest
{
    public string? TagId { get; set; }
    
    public string? ParentTagId { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    public bool? Blocked { get; set; }
}