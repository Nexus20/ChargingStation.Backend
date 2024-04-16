using ChargingStation.Common.Models.ChargePoints.Responses;

namespace ChargingStation.Aggregator.Services.ChargePoints;

public interface IChargePointsHttpService
{
    Task<List<ChargePointResponse>?> GetAsync(IEnumerable<Guid> depotsIds, CancellationToken cancellationToken = default);
}