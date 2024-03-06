using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;
using ChargingStation.OcppTags.Models.Requests;
using ChargingStation.Transactions.Models.Requests;

namespace ChargingStation.Transactions.Specifications;

public class GetTransactionsSpecification : Specification<OcppTransaction>
{
    public GetTransactionsSpecification(GetTransactionsRequest request)
    {
        AddFilters(request);

        if (request.OrderPredicates.Count != 0)
            AddSorting(request.OrderPredicates);
    }

    private void AddFilters(GetTransactionsRequest request)
    {
        if(request.TransactionId.HasValue)
            AddFilter(t => t.TransactionId == request.TransactionId);
        
        // if (!string.IsNullOrEmpty(request.TagId))
        //     AddFilter(с => с.TagId.Contains(request.TagId));
        //
        // if (!string.IsNullOrEmpty(request.ParentTagId))
        //     AddFilter(с => с.ParentTagId != null && с.ParentTagId.Contains(request.ParentTagId));
        //
        // if (request.Blocked.HasValue)
        //     AddFilter(с => с.Blocked == request.Blocked);
        //
        // if (request.ExpiryDate.HasValue)
        //     AddFilter(с => с.ExpiryDate == request.ExpiryDate);
    }
}