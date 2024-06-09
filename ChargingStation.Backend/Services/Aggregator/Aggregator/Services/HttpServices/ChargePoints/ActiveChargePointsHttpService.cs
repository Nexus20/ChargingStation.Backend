using ChargingStation.Common.Models.ChargePoints.Responses;

namespace Aggregator.Services.HttpServices.ChargePoints;

public class ActiveChargePointsHttpService : IActiveChargePointsHttpService
{
    private readonly HttpClient _httpClient;

    public ActiveChargePointsHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ActiveChargePointResponse>?> GetActiveChargePointsAsync(CancellationToken cancellationToken = default)
    {
        var httpResponse = await _httpClient.GetAsync("active", cancellationToken);
        
        httpResponse.EnsureSuccessStatusCode();
        
        return await httpResponse.Content.ReadFromJsonAsync<List<ActiveChargePointResponse>>(cancellationToken: cancellationToken);
    }
}