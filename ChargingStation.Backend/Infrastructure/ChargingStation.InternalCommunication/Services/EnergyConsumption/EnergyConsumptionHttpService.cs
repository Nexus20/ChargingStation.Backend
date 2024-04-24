using System.Net;
using System.Net.Http.Json;
using ChargingStation.Common.Models.DepotEnergyConsumption;

namespace ChargingStation.InternalCommunication.Services.EnergyConsumption;

public class EnergyConsumptionHttpService : IEnergyConsumptionHttpService
{
    private readonly HttpClient _httpClient;

    public EnergyConsumptionHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DepotEnergyConsumptionSettingsResponse?> GetEnergyConsumptionSettingsByDepotAsync(Guid depotId)
    {
        var requestUri = $"getByDepot/{depotId}";
        
        var response = await _httpClient.GetAsync(requestUri);
        
        response.EnsureSuccessStatusCode();
        
        if(response.StatusCode == HttpStatusCode.NoContent)
            return null;
        
        var result = await response.Content.ReadFromJsonAsync<DepotEnergyConsumptionSettingsResponse>();
        
        return result;
    }
}