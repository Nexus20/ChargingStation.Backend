using System.Text;
using ChargingStation.Common.Models.Connectors.Responses;
using Newtonsoft.Json;

namespace ChargingStation.Aggregator.Services.Connectors;

public class ConnectorsHttpService : IConnectorsHttpService
{
    private readonly HttpClient _httpClient;

    public ConnectorsHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ConnectorResponse>?> GetAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default)
    {
        var jsonBody = JsonConvert.SerializeObject(chargePointsIds);
        var requestContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        
        var httpResponse = await _httpClient.PostAsync("GetByChargePoints", requestContent, cancellationToken);
        
        httpResponse.EnsureSuccessStatusCode();
        
        var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        
        return JsonConvert.DeserializeObject<List<ConnectorResponse>>(responseString);
    }
}