using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;

namespace ChargingStation.Transactions.Repositories;

public interface ITransactionRepository : IRepository<OcppTransaction>
{
    Task<OcppTransaction?> GetLastActiveTransactionAsync(string idTag, CancellationToken cancellationToken = default);
}