using ChargingStation.Common.Models.ChargePoints.Responses;

namespace Aggregator.Services.HttpServices.ChargePoints;

public interface IChargePointsHttpService
{
    Task<List<ChargePointResponse>?> GetAsync(IEnumerable<Guid> depotsIds, CancellationToken cancellationToken = default);
}