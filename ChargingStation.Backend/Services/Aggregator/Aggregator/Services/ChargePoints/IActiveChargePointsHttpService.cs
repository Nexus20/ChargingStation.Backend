using ChargingStation.Common.Models.ChargePoints.Responses;

namespace Aggregator.Services.ChargePoints;

public interface IActiveChargePointsHttpService
{
    Task<List<ActiveChargePointResponse>?> GetActiveChargePointsAsync(CancellationToken cancellationToken = default);
}