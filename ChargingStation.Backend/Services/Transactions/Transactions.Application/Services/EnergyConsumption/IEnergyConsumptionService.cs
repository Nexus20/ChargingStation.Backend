using Transactions.Application.Models.EnergyConsumption.Requests;
using Transactions.Application.Models.EnergyConsumption.Responses;

namespace Transactions.Application.Services.EnergyConsumption;

public interface IEnergyConsumptionService
{
    public Task<ChargePointsEnergyConsumptionResponse> GetChargePointsEnergyConsumptionByDepotAsync(GetChargePointsEnergyConsumptionByDepotRequest request, CancellationToken cancellationToken = default);
}