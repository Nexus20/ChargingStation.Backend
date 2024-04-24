using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetDepotEnergyConsumptionSettingsWithDetailsSpecification : Specification<DepotEnergyConsumptionSettings>
{
    public GetDepotEnergyConsumptionSettingsWithDetailsSpecification(Guid id)
    {
        AddFilter(x => x.Id == id);
        AddInclude(nameof(DepotEnergyConsumptionSettings.ChargePointsLimits));
        AddInclude(nameof(DepotEnergyConsumptionSettings.Intervals));
    }
}