using ChargingStation.Common.Models.ChargePoints.Responses;
using Microsoft.AspNetCore.Http.Extensions;

namespace ChargingStation.Aggregator.Services.ChargePoints;

public class ChargePointsHttpService : IChargePointsHttpService
{
    private readonly HttpClient _httpClient;

    public ChargePointsHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ChargePointResponse>?> GetAsync(IEnumerable<Guid> depotsIds, CancellationToken cancellationToken = default)
    {
        
        var queryBuilder = new QueryBuilder { { "depotsIds", depotsIds.Select(x => x.ToString()) } };

        var httpResponse = await _httpClient.GetAsync($"getbydepots{queryBuilder.ToQueryString()}", cancellationToken);

        httpResponse.EnsureSuccessStatusCode();

        return await httpResponse.Content.ReadFromJsonAsync<List<ChargePointResponse>>(cancellationToken: cancellationToken);
    }
}