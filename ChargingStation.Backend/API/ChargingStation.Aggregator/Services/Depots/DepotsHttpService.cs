using ChargingStation.Aggregator.Models.Requests;
using ChargingStation.Aggregator.Models.Responses;
using ChargingStation.Common.Models.General;
using Newtonsoft.Json;

namespace ChargingStation.Aggregator.Services.Depots;

public class DepotsHttpService : IDepotsHttpService
{
    private readonly HttpClient _httpClient;

    public DepotsHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IPagedCollection<DepotAggregatedResponse>?> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync("getall", request, cancellationToken: cancellationToken);
        
        httpResponse.EnsureSuccessStatusCode();
        
        var stringContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
        
        if (string.IsNullOrWhiteSpace(stringContent))
            return null;
        
        return JsonConvert.DeserializeObject<PagedCollection<DepotAggregatedResponse>>(stringContent);
    }
}