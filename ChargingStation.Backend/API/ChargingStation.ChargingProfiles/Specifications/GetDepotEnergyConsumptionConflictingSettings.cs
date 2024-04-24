using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetDepotEnergyConsumptionConflictingSettings : Specification<DepotEnergyConsumptionSettings>
{
    public GetDepotEnergyConsumptionConflictingSettings(Guid depotId, DateTime validFrom, DateTime validTo)
    {
        AddFilter(x => x.DepotId == depotId);
        AddFilter(x => x.ValidFrom >= validFrom || x.ValidTo <= validTo);
    }
}