using ChargingStation.ChargingProfiles.Models.Requests;
using ChargingStation.ChargingProfiles.Models.Responses;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;

namespace ChargingStation.ChargingProfiles.Services;

public interface IChargingProfileService
{
    Task ProcessSetChargingProfileResponseAsync(SetChargingProfileResponse setChargingProfileResponse,
        Guid chargePointId, string ocppMessageId, CancellationToken cancellationToken = default);
    Task SetChargingProfileAsync(SetChargingProfileRequest request, CancellationToken cancellationToken = default);
    Task<IPagedCollection<ChargingProfileResponse>> GetAsync(GetChargingProfilesRequest request, CancellationToken cancellationToken = default);
    Task<ChargingProfileResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChargingProfileResponse> CreateAsync(CreateChargingProfileRequest request, CancellationToken cancellationToken = default);
    Task ClearChargingProfileAsync(ClearChargingProfileRequest request, CancellationToken cancellationToken = default);
    Task ProcessClearChargingProfileResponseAsync(ClearChargingProfileResponse clearChargingProfileResponse, Guid chargePointId, string ocppMessageId, CancellationToken cancellationToken = default);
}