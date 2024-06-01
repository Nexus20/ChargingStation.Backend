using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace ChargePointEmulator.Application.Services;

internal class AuthHttpService : IAuthHttpService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthHttpService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<LoginResponse> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        var request = new LoginRequest
        {
            Email = _configuration["AuthSettings:AuthServiceEmail"]!,
            Password = _configuration["AuthSettings:AuthServicePassword"]!
        };
        
        var response = await _httpClient.PostAsJsonAsync("login", request, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
    }

    private class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    
    internal class LoginResponse
    {
        public required string Token { get; set; }
    }
}