using ChargingStation.Common.Models.ConnectorEnergyConsumption;
using Transactions.Application.Models.EnergyConsumption.Requests;
using Transactions.Application.Models.EnergyConsumption.Responses;

namespace Transactions.Application.Services.EnergyConsumption;

public interface IEnergyConsumptionService
{
    Task<ChargePointsEnergyConsumptionResponse> GetChargePointsEnergyConsumptionByDepotAsync(GetChargePointsEnergyConsumptionByDepotRequest request, CancellationToken cancellationToken = default);

    Task<List<DepotEnergyConsumptionStatisticsResponse>> GetDepotEnergyConsumption(GetDepotEnergyConsumptionStatisticsRequest request, CancellationToken cancellationToken = default);
    Task<List<ConnectorEnergyConsumptionResponse>> GetConnectorsEnergyConsumptionAsync(List<Guid> connectorsIds, CancellationToken cancellationToken = default);
}