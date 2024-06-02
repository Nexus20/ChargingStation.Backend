using ChargingStation.Common.Models.General;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;

namespace UserManagement.API.Services.Users;

public interface IUserService
{
    Task<IPagedCollection<UserResponse>> GetAsync(GetUsersRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponse> GetPersonalInfoAsync(Guid id, CancellationToken cancellationToken = default);
}