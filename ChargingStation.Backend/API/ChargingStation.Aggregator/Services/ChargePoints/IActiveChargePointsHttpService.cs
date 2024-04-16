using ChargingStation.Common.Models.ChargePoints.Responses;

namespace ChargingStation.Aggregator.Services.ChargePoints;

public interface IActiveChargePointsHttpService
{
    Task<List<ActiveChargePointResponse>?> GetActiveChargePointsAsync(CancellationToken cancellationToken = default);
}