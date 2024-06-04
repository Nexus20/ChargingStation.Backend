using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using Transactions.Application.Models.Dtos;
using Transactions.Application.Models.EnergyConsumption.Responses;

namespace Transactions.Application.Repositories.ConnectorMeterValues;

public interface IConnectorMeterValueRepository : IRepository<ConnectorMeterValue>
{
    Task<double> GetTotalEnergyConsumedByDepotAsync(Guid depotId, DateTime validFrom,
        CancellationToken cancellationToken = default);

    Task<double> GetTotalEnergyConsumedByChargePointAsync(Guid chargePointId, DateTime validFrom,
        CancellationToken cancellationToken = default);

    Task<List<SoCDateTime>> GetSoCForTransactionAsync(Guid transactionId, CancellationToken cancellationToken = default);

    Task<List<ChargePointConnectorEnergyConsumptionResponse>> GetChargePointsConnectorsEnergyConsumptionByDepotAsync(
        List<Guid> connectorsIds, DateTime? startTime, DateTime? endTime);
}