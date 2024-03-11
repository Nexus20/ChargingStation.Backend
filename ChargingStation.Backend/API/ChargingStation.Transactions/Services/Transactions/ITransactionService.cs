using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models;
using ChargingStation.Transactions.Models.Requests;
using ChargingStation.Transactions.Models.Responses;

namespace ChargingStation.Transactions.Services.Transactions;

public interface ITransactionService
{
    Task<IPagedCollection<TransactionResponse>> GetAsync(GetTransactionsRequest request, CancellationToken cancellationToken = default);
    Task<TransactionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TransactionResponse> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default);
    Task<TransactionResponse> UpdateAsync(UpdateTransactionRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StartTransactionResponse> ProcessStartTransactionAsync(StartTransactionRequest request, Guid chargePointId, CancellationToken cancellationToken = default);
    Task<StopTransactionResponse> ProcessStopTransactionAsync(StopTransactionRequest request, Guid chargePointId, CancellationToken cancellationToken = default);
}