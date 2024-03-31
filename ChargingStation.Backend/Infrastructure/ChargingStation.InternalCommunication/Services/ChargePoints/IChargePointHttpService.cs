using ChargingStation.Common.Models.Models.Responses;

namespace ChargingStation.InternalCommunication.Services.ChargePoints;

public interface IChargePointHttpService
{
    Task<ChargePointResponse> GetByIdAsync(Guid chargePointId, CancellationToken cancellationToken = default);
}