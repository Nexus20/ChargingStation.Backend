using ChargingStation.ChargePoints.Models.Requests;
using ChargingStation.ChargePoints.Models.Responses;
using ChargingStation.Common.Models;

namespace ChargingStation.ChargePoints.Services;

public interface IChargePointService
{
    Task<IPagedCollection<ChargePointResponse>> GetAsync(GetChargePoint request, CancellationToken cancellationToken = default);
    Task<ChargePointResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChargePointResponse> CreateAsync(CreateChargePoint chargePoint, CancellationToken cancellationToken = default);
    Task<ChargePointResponse> UpdateAsync(UpdateChargePoint chargePoint, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

