using ChargingStation.Common.Models.OcppTags.Responses;

namespace ChargingStation.InternalCommunication.Services.OcppTags;

public interface IOcppTagHttpService
{
    Task<OcppTagResponse?> GetByOcppTagIdAsync(string ocppTagId, CancellationToken cancellationToken = default);
    Task<OcppTagResponse> GetByIdAsync(Guid tagId, CancellationToken cancellationToken);
}