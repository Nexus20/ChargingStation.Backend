using System.Net.Http.Json;

namespace ChargingStation.InternalCommunication.Services.UserManagement;

public class UserHttpService : IUserHttpService
{
    private readonly HttpClient _httpClient;

    public UserHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Guid>> GetUserDepotAccessesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"api/user/{userId}/depots-accesses";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<Guid>>(cancellationToken: cancellationToken);
        return result;
    }
}