using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.TimeZone;
using Depots.Application.Models.Requests;

namespace Depots.Application.Services;

public interface IDepotService
{
    Task<IPagedCollection<DepotResponse>> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default);
    Task<DepotResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DepotResponse> CreateAsync(CreateDepotRequest request, CancellationToken cancellationToken = default);
    Task<DepotResponse> UpdateAsync(UpdateDepotRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IPagedCollection<TimeZoneResponse>> GetTimeZonesAsync(GetTimeZoneRequest request, CancellationToken cancellationToken = default);
}