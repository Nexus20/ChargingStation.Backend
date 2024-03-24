using ChargingStation.OcppTags.Models.Responses;

namespace ChargingStation.Reservations.Services.OcppTags;

public interface IOcppTagHttpService
{
    Task<OcppTagResponse?> GetByOcppTagIdAsync(string ocppTagId, CancellationToken cancellationToken = default);
    Task<OcppTagResponse> GetByIdAsync(Guid tagId, CancellationToken cancellationToken);
}