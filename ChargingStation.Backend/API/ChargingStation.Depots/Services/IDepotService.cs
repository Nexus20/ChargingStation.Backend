using ChargingStation.Depots.Models.Responses;

namespace ChargingStation.Depots.Services;

public interface IDepotService
{
    Task<List<DepotResponse>> GetAsync(CancellationToken cancellationToken = default);
    Task<DepotResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DepotResponse> CreateAsync(DepotResponse depot, CancellationToken cancellationToken = default);
    Task<DepotResponse> UpdateAsync(DepotResponse depot, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}