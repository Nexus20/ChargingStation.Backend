using ChargingStation.Common.Models;
using ChargingStation.Depots.Models.Requests;
using ChargingStation.Depots.Models.Responses;

namespace ChargingStation.Depots.Services;

public interface IDepotService
{
    Task<IPagedCollection<DepotResponse>> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default);
    Task<DepotResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DepotResponse> CreateAsync(DepotResponse depot, CancellationToken cancellationToken = default);
    Task<DepotResponse> UpdateAsync(DepotResponse depot, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}