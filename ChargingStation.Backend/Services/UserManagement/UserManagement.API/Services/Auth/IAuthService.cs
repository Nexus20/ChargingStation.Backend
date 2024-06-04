using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;

namespace UserManagement.API.Services.Auth;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest loginRequest);
    Task RegisterAsync(RegisterRequest registerRequest);
    Task ConfirmRegistration(ConfirmRegistrationRequest confirmRegistrationRequest);
}