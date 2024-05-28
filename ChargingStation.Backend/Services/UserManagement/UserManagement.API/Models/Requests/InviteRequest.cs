namespace UserManagement.API.Models.Requests;

public class InviteRequest
{
    public required DateTime Expiration { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public required Guid DepotId { get; set; }
}