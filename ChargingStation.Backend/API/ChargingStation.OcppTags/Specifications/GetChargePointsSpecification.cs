using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;
using ChargingStation.OcppTags.Models.Requests;

namespace ChargingStation.OcppTags.Specifications;

public class GetOcppTagsSpecification : Specification<OcppTag>
{
    public GetOcppTagsSpecification(GetOcppTagsRequest request)
    {
        AddFilters(request);

        if (request.OrderPredicates.Count != 0)
            AddSorting(request.OrderPredicates);
    }

    private void AddFilters(GetOcppTagsRequest request)
    {
        if (!string.IsNullOrEmpty(request.TagId))
            AddFilter(с => с.TagId.Contains(request.TagId));

        if (!string.IsNullOrEmpty(request.ParentTagId))
            AddFilter(с => с.ParentTagId != null && с.ParentTagId.Contains(request.ParentTagId));
        
        if (request.Blocked.HasValue)
            AddFilter(с => с.Blocked == request.Blocked);
        
        if (request.ExpiryDate.HasValue)
            AddFilter(с => с.ExpiryDate == request.ExpiryDate);
    }
}