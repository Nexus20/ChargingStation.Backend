using System.Text;
using ChargingStation.Common.Models.ConnectorEnergyConsumption;
using Newtonsoft.Json;

namespace Aggregator.Services.HttpServices.EnergyConsumption;

public class EnergyConsumptionHttpService : IEnergyConsumptionHttpService
{
    private readonly HttpClient _httpClient;

    public EnergyConsumptionHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ConnectorEnergyConsumptionResponse>> GetConnectorsEnergyConsumptionAsync(List<Guid> connectorsIds, CancellationToken cancellationToken = default)
    {
        var jsonBody = JsonConvert.SerializeObject(connectorsIds);
        
        var requestContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        
        var httpResponse = await _httpClient.PostAsync("connectors-consumption", requestContent, cancellationToken);
        
        httpResponse.EnsureSuccessStatusCode();
        
        var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        
        return JsonConvert.DeserializeObject<List<ConnectorEnergyConsumptionResponse>>(responseString);
    }
}