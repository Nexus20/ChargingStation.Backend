using ChargingStation.Common.Models.DepotEnergyConsumption;
using EnergyConsumption.Application.Models.Requests;
using EnergyConsumption.Application.Models.Responses;

namespace EnergyConsumption.Application.Services;

public interface IEnergyConsumptionSettingsService
{
    Task<Guid> SetEnergyConsumptionSettingsAsync(SetDepotEnergyConsumptionSettingsRequest request, CancellationToken cancellationToken = default);
    Task<DepotEnergyConsumptionSettingsResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DepotEnergyConsumptionSettingsResponse?> GetByDepotIdAsync(Guid depotId, CancellationToken cancellationToken = default);
    Task<DepotEnergyConsumptionSettingsResponse?> GetByChargingStationIdAsync(Guid chargingStationId, CancellationToken cancellationToken = default);
    Task<DepotEnergyConsumptionSettingsStatisticsResponse?> GetDepotEnergyConsumptionStatisticsAsync(GetDepotEnergyConsumptionSettingsStatisticsRequest request, CancellationToken cancellationToken = default);
}