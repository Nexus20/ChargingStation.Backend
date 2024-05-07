namespace OcppTags.Application.Models.Requests;

public class UpdateOcppTagRequest
{
    public Guid Id { get; set; }
    
    public required string TagId { get; set; }
    
    public string? ParentTagId { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
}