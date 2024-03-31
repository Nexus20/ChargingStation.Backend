using System.Net.Http.Json;
using ChargingStation.Common.Models.Transactions.Responses;

namespace ChargingStation.InternalCommunication.Services.Transactions;

public class TransactionHttpService : ITransactionHttpService
{
    private readonly HttpClient _httpClient;

    public TransactionHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TransactionResponse> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"api/Transaction/{transactionId}";
        var result = await _httpClient.GetAsync(requestUri, cancellationToken);
        result.EnsureSuccessStatusCode();
        
        var response = await result.Content.ReadFromJsonAsync<TransactionResponse>(cancellationToken: cancellationToken);
        return response ?? throw new InvalidOperationException();
    }
}