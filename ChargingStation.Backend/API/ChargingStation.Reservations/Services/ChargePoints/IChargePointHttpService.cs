using ChargingStation.ChargePoints.Models.Responses;

namespace ChargingStation.Reservations.Services.ChargePoints;

public interface IChargePointHttpService
{
    Task<ChargePointResponse> GetByIdAsync(Guid chargePointId, CancellationToken cancellationToken = default);
}