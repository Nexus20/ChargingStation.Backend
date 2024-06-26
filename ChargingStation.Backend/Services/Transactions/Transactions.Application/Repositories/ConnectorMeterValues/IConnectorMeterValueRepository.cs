using ChargingStation.Common.Models.ConnectorEnergyConsumption;
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
        List<Guid> connectorsIds, DateTime? startTime, DateTime? endTime,
        CancellationToken cancellationToken = default);
    
    Task<List<DepotEnergyConsumptionStatisticsResponse>> GetDepotEnergyConsumptionAsync(List<Guid> connectorsIds,
        TimeSpan aggregationInterval, DateTime? startTime, DateTime? endTime,
        CancellationToken cancellationToken = default);

    Task<List<ConnectorEnergyConsumptionResponse>> GetConnectorsEnergyConsumptionAsync(List<Guid> connectorsIds, CancellationToken cancellationToken = default);
}