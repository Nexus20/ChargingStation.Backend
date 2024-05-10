using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetChargingProfileByNumericIdSpecification : Specification<ChargingProfile>
{
    public GetChargingProfileByNumericIdSpecification(int id)
    {
        AddFilter(x => x.ChargingProfileId == id);
    }
}