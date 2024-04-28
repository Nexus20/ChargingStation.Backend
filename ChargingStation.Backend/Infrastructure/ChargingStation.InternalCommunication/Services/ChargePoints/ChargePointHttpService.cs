using System.Net.Http.Json;
using System.Text;
using ChargingStation.Common.Models.ChargePoints.Requests;
using ChargingStation.Common.Models.ChargePoints.Responses;
using Newtonsoft.Json;

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

    public async Task<List<ChargePointResponse>?> GetByIdAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default)
    {
        const string requestUri = "api/ChargePoint/GetByIds";
        var requestContent = new StringContent(JsonConvert.SerializeObject(chargePointsIds), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(requestUri, requestContent, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<List<ChargePointResponse>>(cancellationToken: cancellationToken);
        return result;
    }

    public async Task ChangeAvailabilityAsync(ChangeChargePointAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        const string requestUri = "api/ChargePoint/changeavailability";
        
        var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(requestUri, requestContent, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }
}