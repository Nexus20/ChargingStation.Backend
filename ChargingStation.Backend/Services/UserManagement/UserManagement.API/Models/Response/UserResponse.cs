namespace UserManagement.API.Models.Response;

public class UserResponse
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? OcppTagId { get; set; }
    public List<Guid>? DepotsIds { get; set; }
}