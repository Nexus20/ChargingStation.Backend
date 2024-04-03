using System.Net.Http.Json;
using ChargingStation.Common.Models.Reservations.Requests;

namespace ChargingStation.InternalCommunication.Services.Reservations;

public class ReservationHttpService : IReservationHttpService
{
    private readonly HttpClient _httpClient;

    public ReservationHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task UseReservationAsync(UseReservationRequest request, CancellationToken cancellationToken = default)
    {
        const string requestUri = "api/Reservation/Use";
        var response = await _httpClient.PostAsJsonAsync(requestUri, request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }
}