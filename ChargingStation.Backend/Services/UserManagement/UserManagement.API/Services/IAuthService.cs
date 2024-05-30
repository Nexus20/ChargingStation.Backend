﻿using ChargingStation.Common.Models.General;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;

namespace UserManagement.API.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest loginRequest);
        Task<TokenResponse> RegisterAsync(RegisterRequest registerRequest);
        string GenerateInvitationToken(InviteRequest inviteRequest);
        Task SendInvitationEmailAsync(InviteRequest inviteRequest, string invitationLink);
        Task ConfirmInvite(string token);
        Task<IPagedCollection<UserResponse>> GetUsers(GetUserRequest request, CancellationToken cancellationToken);
    }
}
