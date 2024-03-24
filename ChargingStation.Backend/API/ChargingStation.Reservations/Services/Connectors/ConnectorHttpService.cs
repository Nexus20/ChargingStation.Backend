using ChargingStation.Connectors.Models.Requests;
using ChargingStation.Connectors.Models.Responses;

namespace ChargingStation.Reservations.Services.Connectors;

public class ConnectorHttpService : IConnectorHttpService
{
    private readonly HttpClient _httpClient;

    public ConnectorHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ConnectorResponse> GetOrCreateConnectorAsync(GetOrCreateConnectorRequest request, CancellationToken cancellationToken = default)
    {
        const string requestUri = "api/Connector/GetOrCreate";
        var response = await _httpClient.PostAsJsonAsync(requestUri, request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ConnectorResponse>(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException();
    }

    public async Task UpdateConnectorStatusAsync(UpdateConnectorStatusRequest updateStatusRequest, CancellationToken cancellationToken)
    {
        const string requestUri = "api/Connector/UpdateStatus";
        var response = await _httpClient.PostAsJsonAsync(requestUri, updateStatusRequest, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task<ConnectorResponse> GetByIdAsync(Guid connectorId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"api/Connector/{connectorId}";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ConnectorResponse>(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException();
    }

    public Task<ConnectorResponse> GetAsync(Guid chargePointId, int connectorId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ConnectorStatusResponse> GetConnectorStatusAsync(Guid chargePointId, int connectorId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ConnectorStatusResponse> GetConnectorStatusAsync(Guid connectorId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}