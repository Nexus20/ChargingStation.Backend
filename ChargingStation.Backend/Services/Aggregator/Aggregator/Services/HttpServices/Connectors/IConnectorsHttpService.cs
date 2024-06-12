using ChargingStation.Common.Models.Connectors.Responses;

namespace Aggregator.Services.HttpServices.Connectors;

public interface IConnectorsHttpService
{
    Task<List<ConnectorResponse>?> GetAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default);
}