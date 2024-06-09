using ChargingStation.Common.Models.ChargePoints.Responses;

namespace Aggregator.Services.HttpServices.ChargePoints;

public interface IActiveChargePointsHttpService
{
    Task<List<ActiveChargePointResponse>?> GetActiveChargePointsAsync(CancellationToken cancellationToken = default);
}