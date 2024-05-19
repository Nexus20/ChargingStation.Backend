using System.Net.Http.Json;
using ChargePointEmulator.Application.Models;

namespace ChargePointEmulator.Application.Services;

public class ReservationHttpService : IReservationHttpService
{
    private readonly HttpClient _httpClient;

    public ReservationHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task CreateReservationAsync(CreateReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("", request, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}