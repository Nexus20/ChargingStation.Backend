using ChargingStation.ChargingProfiles.Models.Requests;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargingProfiles.Specifications;

public class GetChargingProfilesSpecification : Specification<ChargingProfile>
{
    public GetChargingProfilesSpecification(GetChargingProfilesRequest request)
    {
        AddFilters(request);
        AddInclude($"{nameof(ChargingProfile.ChargingSchedulePeriods)}");
    }
    
    private void AddFilters(GetChargingProfilesRequest request)
    {
        if (request.StackLevel.HasValue) 
            AddFilter(x => x.StackLevel == request.StackLevel);

        if (request.ValidFrom.HasValue) 
            AddFilter(x => x.ValidFrom == request.ValidFrom);

        if (request.ValidTo.HasValue) 
            AddFilter(x => x.ValidTo == request.ValidTo);

        if (request.RecurrencyKind.HasValue) 
            AddFilter(x => x.RecurrencyKind == request.RecurrencyKind);

        if (request.ChargingProfilePurpose.HasValue) 
            AddFilter(x => x.ChargingProfilePurpose == request.ChargingProfilePurpose);

        if (request.ChargingProfileKind.HasValue) 
            AddFilter(x => x.ChargingProfileKind == request.ChargingProfileKind);

        if (request.Duration.HasValue) 
            AddFilter(x => x.Duration == request.Duration);

        if (request.StartSchedule.HasValue) 
            AddFilter(x => x.StartSchedule == request.StartSchedule);

        if (request.SchedulingUnit.HasValue) 
            AddFilter(x => x.SchedulingUnit == request.SchedulingUnit);

        if (request.MinChargingRate.HasValue) 
            AddFilter(x => x.MinChargingRate == request.MinChargingRate);
        
        if (!string.IsNullOrEmpty(request.Name))
            AddFilter(x => x.Name.Contains(request.Name));
    }
}