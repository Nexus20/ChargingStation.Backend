using ChargingStation.Depots.Models.Requests;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.Depots.Specifications;


internal class GetDepotsSpecification : Specification<Depot>
{
    public GetDepotsSpecification(GetDepotsRequest request)
    {
        AddFilters(request);
        
        if(request.OrderPredicates.Count != 0)
            AddSorting(request.OrderPredicates);
    }

    private void AddFilters(GetDepotsRequest request)
    {
        if (!string.IsNullOrEmpty(request.Name)) 
            AddFilter(d => d.Name.Contains(request.Name));
        
        if (!string.IsNullOrEmpty(request.Country)) 
            AddFilter(d => d.Country.Contains(request.Country));
        
        if (!string.IsNullOrEmpty(request.City)) 
            AddFilter(d => d.City.Contains(request.City));
        
        if (!string.IsNullOrEmpty(request.Street)) 
            AddFilter(d => d.Street.Contains(request.Street));
        
        if (!string.IsNullOrEmpty(request.Building)) 
            AddFilter(d => d.Building.Contains(request.Building));
        
        if (request.Status.HasValue) 
            AddFilter(d => d.Status == request.Status);
        
        if (request.CreatedAt.HasValue) 
            AddFilter(d => d.CreatedAt == request.CreatedAt);
        
        if (request.UpdatedAt.HasValue) 
            AddFilter(d => d.UpdatedAt == request.UpdatedAt);
    }
}