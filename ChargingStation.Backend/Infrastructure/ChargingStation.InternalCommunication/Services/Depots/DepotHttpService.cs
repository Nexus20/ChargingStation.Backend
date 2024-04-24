using System.Net.Http.Json;
using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General;
using Newtonsoft.Json;

namespace ChargingStation.InternalCommunication.Services.Depots;

public class DepotHttpService : IDepotHttpService
{
    private readonly HttpClient _httpClient;

    public DepotHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DepotResponse?> GetByIdAsync(Guid depotId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"api/depot/{depotId}";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<DepotResponse>(cancellationToken: cancellationToken);
        return result;
    }
    
    public async Task<IPagedCollection<DepotResponse>?> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default)
    {
        const string requestUri = "api/depot/getall";
        var httpResponse = await _httpClient.PostAsJsonAsync(requestUri, request, cancellationToken: cancellationToken);
        
        httpResponse.EnsureSuccessStatusCode();
        
        var stringContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
        
        if (string.IsNullOrWhiteSpace(stringContent))
            return null;
        
        return JsonConvert.DeserializeObject<PagedCollection<DepotResponse>>(stringContent);
    }
}