using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;

namespace UserManagement.API.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest loginRequest);
        Task<TokenResponse> RegisterAsync(RegisterRequest registerRequest);
    }
}
