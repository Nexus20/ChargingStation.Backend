using ChargePointEmulator.Application.Models;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.Common.Models.General;

namespace ChargePointEmulator.Application.Services;

public interface IChargePointService
{
    Task<IPagedCollection<ChargePointSimulatorResponse>> GetAsync(GetChargePointRequest request,
        CancellationToken cancellationToken = default);
    Task<ChargePointSimulatorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddSimulatorAsync(Guid id, CancellationToken cancellationToken = default);
}

