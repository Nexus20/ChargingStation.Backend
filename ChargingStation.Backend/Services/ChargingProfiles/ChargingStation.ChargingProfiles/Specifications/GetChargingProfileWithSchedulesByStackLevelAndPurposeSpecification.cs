using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetChargingProfileWithSchedulesByStackLevelAndPurposeSpecification : Specification<ChargingProfile>
{
    public GetChargingProfileWithSchedulesByStackLevelAndPurposeSpecification(int stackLevel, ChargingProfilePurpose chargingProfilePurpose)
    {
        AddFilter(x => x.StackLevel == stackLevel && x.ChargingProfilePurpose == chargingProfilePurpose);
        AddInclude(nameof(ChargingProfile.ChargingSchedulePeriods));
    }
}