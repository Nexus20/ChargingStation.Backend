using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;
using UserManagement.API.Specifications;

namespace UserManagement.API.Services.Users;

public class UserService : IUserService
{
    private readonly IRepository<ApplicationUser> _applicationUserRepository;
    private readonly IMapper _mapper;

    public UserService(IRepository<ApplicationUser> applicationUserRepository, IMapper mapper)
    {
        _applicationUserRepository = applicationUserRepository;
        _mapper = mapper;
    }

    public async Task<IPagedCollection<UserResponse>> GetAsync(GetUsersRequest request,
        CancellationToken cancellationToken = default)
    {
        var specification = new GetUsersSpecification(request);

        var users = await _applicationUserRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);

        if (!users.Collection.Any())
            return PagedCollection<UserResponse>.Empty;

        var result = _mapper.Map<IPagedCollection<UserResponse>>(users);
        return result;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _applicationUserRepository.GetByIdAsync(id, cancellationToken);
        
        if (user is null)
            throw new NotFoundException($"User with id {id} not found");

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> GetPersonalInfoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _applicationUserRepository.GetByIdAsync(id, cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }
}