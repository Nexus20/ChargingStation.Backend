using ChargingStation.Common.Models.OcppTags.Responses;

namespace ChargePointEmulator.Application.Services;

public interface IOcppTagService
{
    Task<OcppTagResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<OcppTagResponse>> GetAsync(CancellationToken cancellationToken = default);
}