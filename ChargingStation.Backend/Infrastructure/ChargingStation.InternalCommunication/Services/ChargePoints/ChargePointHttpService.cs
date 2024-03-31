using System.Net.Http.Json;
using ChargingStation.Common.Models.Models.Responses;

namespace ChargingStation.InternalCommunication.Services.ChargePoints;

public class ChargePointHttpService : IChargePointHttpService
{
    private readonly HttpClient _httpClient;

    public ChargePointHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ChargePointResponse> GetByIdAsync(Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"api/ChargePoint/{chargePointId}";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ChargePointResponse>(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException();
    }
}