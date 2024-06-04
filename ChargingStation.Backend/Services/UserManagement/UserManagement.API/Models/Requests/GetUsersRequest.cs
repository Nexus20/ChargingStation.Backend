using ChargingStation.Common.Models.General.Requests;

namespace UserManagement.API.Models.Requests;

public class GetUsersRequest : BaseCollectionRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Guid? DepotId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? OcppTagId { get; set; }
}