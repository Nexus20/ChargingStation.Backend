namespace UserManagement.API.Models.Requests;

 public class DeleteUserWitDepotRequest
 {
     public required Guid DepotId { get; set; }
    public required Guid UserId { get; set; }
}
