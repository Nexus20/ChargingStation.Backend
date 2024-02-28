using ChargingStation.Common.Models;
using ChargingStation.OcppTags.Models.Requests;
using ChargingStation.OcppTags.Models.Responses;

namespace ChargingStation.OcppTags.Services;

public interface IOcppTagService
{
    Task<IPagedCollection<OcppTagResponse>> GetAsync(GetOcppTagsRequest request, CancellationToken cancellationToken = default);
    Task<OcppTagResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OcppTagResponse?> GetByOcppTagIdAsync(string ocppTagId, CancellationToken cancellationToken = default);
    Task<OcppTagResponse> CreateAsync(CreateOcppTagRequest request, CancellationToken cancellationToken = default);
    Task<OcppTagResponse> UpdateAsync(UpdateOcppTagRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}