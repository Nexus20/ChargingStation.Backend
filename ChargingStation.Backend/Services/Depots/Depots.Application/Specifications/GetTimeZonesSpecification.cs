using ChargingStation.Infrastructure.Specifications;
using Depots.Application.Models.Requests;
using TimeZone = ChargingStation.Domain.Entities.TimeZone;

namespace Depots.Application.Specifications;

public class GetTimeZonesSpecification : Specification<TimeZone>
{
    public GetTimeZonesSpecification(GetTimeZoneRequest request)
    {
        AddFilters(request);

        if (request.OrderPredicates.Count != 0)
            AddSorting(request.OrderPredicates);
    }

    private void AddFilters(GetTimeZoneRequest request)
    {
        if (!string.IsNullOrEmpty(request.DisplayName))
            AddFilter(d => d.DisplayName.Contains(request.DisplayName));

        if (!string.IsNullOrEmpty(request.BaseUtcOffset.ToString()))
            AddFilter(d => d.BaseUtcOffset == request.BaseUtcOffset);

        if (!string.IsNullOrEmpty(request.IanaId))
            AddFilter(d => d.IanaId.Contains(request.IanaId));

        if (!string.IsNullOrEmpty(request.WindowsId))
            AddFilter(d => d.WindowsId.Contains(request.WindowsId));
    }
}

