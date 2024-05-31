namespace OcppTags.Application.Models.Requests;

public class CreateOcppTagRequest
{
    public required string TagId { get; set; }
    
    public string? ParentTagId { get; set; }
    public Guid? ApplicationUserId { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
}