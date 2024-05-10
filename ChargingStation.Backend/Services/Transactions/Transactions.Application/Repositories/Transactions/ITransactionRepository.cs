using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;

namespace Transactions.Application.Repositories.Transactions;

public interface ITransactionRepository : IRepository<OcppTransaction>
{
    Task<OcppTransaction?> GetLastActiveTransactionAsync(string idTag, CancellationToken cancellationToken = default);
}