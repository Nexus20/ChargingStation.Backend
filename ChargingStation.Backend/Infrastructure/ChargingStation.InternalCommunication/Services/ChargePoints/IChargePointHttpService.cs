using ChargingStation.Common.Models.ChargePoints.Responses;

namespace ChargingStation.InternalCommunication.Services.ChargePoints;

public interface IChargePointHttpService
{
    Task<ChargePointResponse> GetByIdAsync(Guid chargePointId, CancellationToken cancellationToken = default);
    Task<List<ChargePointResponse>?> GetByIdAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default);
}