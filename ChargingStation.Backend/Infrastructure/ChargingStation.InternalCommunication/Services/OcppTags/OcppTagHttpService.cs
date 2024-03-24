using System.Net;
using System.Net.Http.Json;
using ChargingStation.Common.Models.OcppTags.Responses;

namespace ChargingStation.InternalCommunication.Services.OcppTags;

public class OcppTagHttpService : IOcppTagHttpService
{
    private readonly HttpClient _httpClient;

    public OcppTagHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<OcppTagResponse?> GetByOcppTagIdAsync(string ocppTagId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"api/OcppTag/GetByTagId/{ocppTagId}";
        var result = await _httpClient.GetAsync(requestUri, cancellationToken);
        result.EnsureSuccessStatusCode();
        
        if(result.StatusCode == HttpStatusCode.NoContent)
            return null;
        
        var response = await result.Content.ReadFromJsonAsync<OcppTagResponse>(cancellationToken: cancellationToken);
        return response;
    }
    
    public async Task<OcppTagResponse> GetByIdAsync(Guid tagId, CancellationToken cancellationToken)
    {
        var requestUri = $"api/OcppTag/{tagId}";
        var result = await _httpClient.GetAsync(requestUri, cancellationToken);
        result.EnsureSuccessStatusCode();
        
        var response = await result.Content.ReadFromJsonAsync<OcppTagResponse>(cancellationToken: cancellationToken);
        return response ?? throw new InvalidOperationException();
    }
}