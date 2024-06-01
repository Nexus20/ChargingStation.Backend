using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;
using UserManagement.API.Models.Requests;

namespace UserManagement.API.Specifications;

public class GetUsersSpecification : Specification<ApplicationUser>
{
    public GetUsersSpecification(GetUserRequest request)
    {
        AddInclude(nameof(ApplicationUser.ApplicationUserDepots));
        AddFilters(request);

        if (request.OrderPredicates.Count != 0)
            AddSorting(request.OrderPredicates);
    }

    private void AddFilters(GetUserRequest request)
    {
        if (!string.IsNullOrEmpty(request.FirstName))
            AddFilter(d => d.FirstName.Contains(request.FirstName));

        if (!string.IsNullOrEmpty(request.LastName))
            AddFilter(d => d.LastName.Contains(request.LastName));

        if (!string.IsNullOrEmpty(request.Email))
            AddFilter(d => d.Email.Contains(request.Email));

        if (!string.IsNullOrEmpty(request.Phone))
            AddFilter(d => d.Phone.Contains(request.Phone));

        if (request.CreatedAt.HasValue)
            AddFilter(d => d.CreatedAt == request.CreatedAt);

        if (request.UpdatedAt.HasValue)
            AddFilter(d => d.UpdatedAt == request.UpdatedAt);
    }
}