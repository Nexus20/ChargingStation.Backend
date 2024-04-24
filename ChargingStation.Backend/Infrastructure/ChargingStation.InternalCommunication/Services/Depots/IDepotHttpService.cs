using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General;

namespace ChargingStation.InternalCommunication.Services.Depots;

public interface IDepotHttpService
{
    Task<IPagedCollection<DepotResponse>?> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default);
    Task<DepotResponse?> GetByIdAsync(Guid depotId, CancellationToken cancellationToken = default);
}