using ChargingStation.OcppTags.Models.Responses;

namespace ChargingStation.Transactions.Services.OcppTags;

public interface IOcppTagHttpService
{
    Task<OcppTagResponse?> GetByOcppTagIdAsync(string ocppTagId, CancellationToken cancellationToken = default);
}