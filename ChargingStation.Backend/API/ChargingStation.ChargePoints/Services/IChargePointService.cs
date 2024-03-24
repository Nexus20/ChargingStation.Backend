using ChargingStation.ChargePoints.Models.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.Models.Responses;

namespace ChargingStation.ChargePoints.Services;

public interface IChargePointService
{
    Task<IPagedCollection<ChargePointResponse>> GetAsync(GetChargePointRequest request, CancellationToken cancellationToken = default);
    Task<ChargePointResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChargePointResponse> CreateAsync(CreateChargePointRequest createChargePointRequest, CancellationToken cancellationToken = default);
    Task<ChargePointResponse> UpdateAsync(UpdateChargePointRequest updateChargePointRequest, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task ResetAsync(ResetChargePointRequest request, CancellationToken cancellationToken);
}

