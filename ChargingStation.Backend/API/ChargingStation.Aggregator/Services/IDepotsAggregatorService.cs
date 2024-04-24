using ChargingStation.Aggregator.Models.Responses;
using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.General;

namespace ChargingStation.Aggregator.Services;

public interface IDepotsAggregatorService
{
    Task<IPagedCollection<DepotAggregatedResponse>> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default);
    Task<DepotAggregatedDetailsResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}