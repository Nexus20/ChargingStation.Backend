using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;

namespace ChargingStation.Transactions.Repositories.ConnectorMeterValues;

public interface IConnectorMeterValueRepository : IRepository<ConnectorMeterValue>
{
    Task<double> GetTotalEnergyConsumedByDepotAsync(Guid depotId, DateTime validFrom,
        CancellationToken cancellationToken = default);

    Task<double> GetTotalEnergyConsumedByChargePointAsync(Guid chargePointId, DateTime validFrom,
        CancellationToken cancellationToken = default);
}