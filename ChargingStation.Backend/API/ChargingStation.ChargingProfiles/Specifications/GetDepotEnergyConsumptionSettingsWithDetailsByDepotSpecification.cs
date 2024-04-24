using ChargingStation.Common.Models.General.Requests;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetDepotEnergyConsumptionSettingsWithDetailsByDepotSpecification : Specification<DepotEnergyConsumptionSettings>
{
    public GetDepotEnergyConsumptionSettingsWithDetailsByDepotSpecification(Guid depotId)
    {
        AddFilter(x => x.DepotId == depotId);
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