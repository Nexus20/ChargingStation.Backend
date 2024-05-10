using ChargingStation.Common.Models.General.Requests;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace EnergyConsumption.Application.Specifications;

public class GetDepotEnergyConsumptionSettingsWithDetailsByChargingStationSpecification : Specification<DepotEnergyConsumptionSettings>
{
    public GetDepotEnergyConsumptionSettingsWithDetailsByChargingStationSpecification(Guid chargingStationId)
    {
        AddFilter(x => x.ChargePointsLimits!.Any(y => y.ChargePointId == chargingStationId));
        AddFilter(x => x.ValidFrom <= DateTime.UtcNow && x.ValidTo >= DateTime.UtcNow);
        
        AddInclude(nameof(DepotEnergyConsumptionSettings.ChargePointsLimits));
        AddInclude(nameof(DepotEnergyConsumptionSettings.Intervals));
        
        AddSorting(new List<OrderPredicate>
        {
            new()
            {
                PropertyName = nameof(DepotEnergyConsumptionSettings.CreatedAt),
                OrderDirection = OrderDirection.Descending
            }
        });
    }
}