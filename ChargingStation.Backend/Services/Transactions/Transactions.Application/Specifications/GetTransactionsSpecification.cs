using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;
using Transactions.Application.Models.Requests;

namespace Transactions.Application.Specifications;

public class GetTransactionsSpecification : Specification<OcppTransaction>
{
    public GetTransactionsSpecification(Guid transactionId)
    {
        AddInclude($"{nameof(OcppTransaction.Reservation)}");
        AddFilter(t => t.Id == transactionId);
    }
    
    public GetTransactionsSpecification(GetTransactionsRequest request)
    {
        AddInclude($"{nameof(OcppTransaction.Reservation)}");
        AddFilters(request);

        if (request.OrderPredicates.Count != 0)
            AddSorting(request.OrderPredicates);
    }

    private void AddFilters(GetTransactionsRequest request)
    {
        if(request.TransactionId.HasValue)
            AddFilter(t => t.TransactionId == request.TransactionId);
        
        if(request.StartTime.HasValue)
            AddFilter(t => t.StartTime == request.StartTime);
        
        if(request.StopTime.HasValue)
            AddFilter(t => t.StopTime == request.StopTime);
    }
}