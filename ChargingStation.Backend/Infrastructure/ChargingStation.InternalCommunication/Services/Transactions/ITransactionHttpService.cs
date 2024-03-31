using ChargingStation.Common.Models.Transactions.Responses;

namespace ChargingStation.InternalCommunication.Services.Transactions;

public interface ITransactionHttpService
{
    Task<TransactionResponse> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
}