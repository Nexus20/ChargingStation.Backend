using ChargingStation.Common.Models.General;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;

namespace UserManagement.API.Services.Users;

public interface IUserService
{
    Task<IPagedCollection<UserResponse>> GetAsync(GetUsersRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponse> GetPersonalInfoAsync(Guid id, CancellationToken cancellationToken = default);
    string GenerateInvitationToken(InviteRequest inviteRequest);
    Task SendInvitationEmailAsync(InviteRequest inviteRequest, string invitationLink);
    Task ConfirmInvite(string token);
    Task<UserResponse> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteUserFromDepotAsync(DeleteUserFromDepotRequest request, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetUserDepotsAccesses(Guid userId, CancellationToken cancellationToken);
}