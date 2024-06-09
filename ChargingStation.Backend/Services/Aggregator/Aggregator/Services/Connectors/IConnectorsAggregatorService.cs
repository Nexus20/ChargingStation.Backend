using Aggregator.Models.Responses;

namespace Aggregator.Services.Connectors;

public interface IConnectorsAggregatorService
{
    Task<List<ConnectorAggregatedResponse>> GetByChargePointsIdsAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default);
}