using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetChargingProfileWithSchedulesSpecification : Specification<ChargingProfile>
{
    public GetChargingProfileWithSchedulesSpecification(Guid id)
    {
        AddFilter(x => x.Id == id);
        AddInclude(nameof(ChargingProfile.ChargingSchedulePeriods));
    }
}