using ChargingStation.Common.Models.ChargePoints.Requests;
using ChargingStation.Common.Models.ChargePoints.Responses;

namespace ChargingStation.InternalCommunication.Services.ChargePoints;

public interface IChargePointHttpService
{
    Task<ChargePointResponse> GetByIdAsync(Guid chargePointId, CancellationToken cancellationToken = default);
    Task<List<ChargePointResponse>?> GetByIdsAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default);
    Task ChangeAvailabilityAsync(ChangeChargePointAvailabilityRequest request, CancellationToken cancellationToken = default);
}