﻿using ChargePoints.Application.Models.Requests;
using ChargingStation.Common.Models.ChargePoints.Requests;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.Common.Models.General;

namespace ChargePoints.Application.Services;

public interface IChargePointService
{
    Task<IPagedCollection<ChargePointResponse>> GetAsync(GetChargePointRequest request, CancellationToken cancellationToken = default);
    Task<ChargePointResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChargePointResponse> CreateAsync(CreateChargePointRequest createChargePointRequest, CancellationToken cancellationToken = default);
    Task<ChargePointResponse> UpdateAsync(UpdateChargePointRequest updateChargePointRequest, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task ResetAsync(ResetChargePointRequest request, CancellationToken cancellationToken);
    Task<List<ChargePointResponse>> GetByDepotsIdsAsync(List<Guid> depotsIds, CancellationToken cancellationToken = default);
    Task<List<ChargePointResponse>> GetByIdsAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default);
    Task ChangeAvailabilityAsync(ChangeChargePointAvailabilityRequest request, CancellationToken cancellationToken = default);
}

