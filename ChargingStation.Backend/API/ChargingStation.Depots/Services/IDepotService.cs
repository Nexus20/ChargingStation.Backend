using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Depots.Models.Requests;

namespace ChargingStation.Depots.Services;

public interface IDepotService
{
    Task<IPagedCollection<DepotResponse>> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default);
    Task<DepotResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DepotResponse> CreateAsync(CreateDepotRequest request, CancellationToken cancellationToken = default);
    Task<DepotResponse> UpdateAsync(UpdateDepotRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}