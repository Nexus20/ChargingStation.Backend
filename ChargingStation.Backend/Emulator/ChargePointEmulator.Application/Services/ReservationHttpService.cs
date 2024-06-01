using System.Net.Http.Headers;
using System.Net.Http.Json;
using ChargePointEmulator.Application.Models;

namespace ChargePointEmulator.Application.Services;

internal class ReservationHttpService : IReservationHttpService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthHttpService _authHttpService;

    public ReservationHttpService(HttpClient httpClient, IAuthHttpService authHttpService)
    {
        _httpClient = httpClient;
        _authHttpService = authHttpService;
    }

    public async Task CreateReservationAsync(CreateReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        var loginResponse = await _authHttpService.AuthenticateAsync(cancellationToken);
        var token = loginResponse.Token;
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsJsonAsync("", request, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}