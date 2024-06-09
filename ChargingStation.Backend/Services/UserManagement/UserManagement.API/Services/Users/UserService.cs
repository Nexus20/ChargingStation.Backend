using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Rbac;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Identity;
using ChargingStation.Infrastructure.Repositories;
using ChargingStation.Mailing.Messages;
using ChargingStation.Mailing.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;
using UserManagement.API.Specifications;
using UserManagement.API.Utility;

namespace UserManagement.API.Services.Users;

public class UserService : IUserService
{
    private readonly IRepository<ApplicationUser> _applicationUserRepository;
    private readonly IRepository<ApplicationUserDepot> _applicationUserDepotRepository;
    private readonly UserManager<InfrastructureUser> _userManager;
    private readonly IMapper _mapper;
    private readonly JwtHandler _jwtHandler;
    private readonly IEmailService _emailService;

    public UserService(IRepository<ApplicationUser> applicationUserRepository, UserManager<InfrastructureUser> userManager,
        JwtHandler jwtHandler, IMapper mapper, IEmailService emailService, IRepository<ApplicationUserDepot> applicationUserDepotRepository)
    {
        _applicationUserRepository = applicationUserRepository;
        _userManager = userManager;
        _mapper = mapper;
        _jwtHandler = jwtHandler;
        _emailService = emailService;
        _applicationUserDepotRepository = applicationUserDepotRepository;
    }

    public async Task<IPagedCollection<UserResponse>> GetAsync(GetUsersRequest request,
        CancellationToken cancellationToken = default)
    {
        var specification = new GetUsersSpecification(request);

        var applicationUsers= await _applicationUserRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);
        var infrastructureUsers = await _userManager.Users
            .Where(u => applicationUsers.Collection.Select(au => au.Id).Contains(u.ApplicationUserId))
            .Include(u => u.UserRoles)!
            .ThenInclude(ur => ur.Role)
            .ToListAsync(cancellationToken);

        if (!applicationUsers.Collection.Any())
            return PagedCollection<UserResponse>.Empty;

        var result = _mapper.Map<IPagedCollection<UserResponse>>(applicationUsers);

        var userRolesDict = infrastructureUsers
            .Where(infraUser => infraUser.UserRoles != null && infraUser.UserRoles.Any())
            .Select(infraUser => new
            {
                ApplicationUserId = infraUser.ApplicationUserId,
                Roles = infraUser.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .ToDictionary(x => x.ApplicationUserId, x => x.Roles);

        result.Collection.ToList().ForEach(userResponse =>
        {
            if (userRolesDict.TryGetValue(userResponse.Id, out var roles))
            {
                userResponse.Roles = roles;
            }
        });

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

    public string GenerateInvitationToken(InviteRequest inviteRequest)
    {
        var invitationToken = _jwtHandler.GenerateInviteToken(inviteRequest);

        return invitationToken;
    }

    public async Task SendInvitationEmailAsync(InviteRequest inviteRequest, string invitationLink)
    {
        var emailMessage = new InvitationEmailMessage(invitationLink, inviteRequest.Role);

        await _emailService.SendMessageAsync(emailMessage, inviteRequest.Email);
    }

    public async Task ConfirmInvite(string token)
    {
        var principal = _jwtHandler.ValidateToken(token);

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var depotId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(depotId))
        {
            throw new BadRequestException("Invalid token data");
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), email);
        }

        var userDepot = new ApplicationUserDepot
        {
            ApplicationUserId = user.ApplicationUserId,
            DepotId = Guid.Parse(depotId)
        };

        var roles = principal.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            if (!roles.Contains(role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
        }

        foreach (var role in roles)
        {
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        await _applicationUserDepotRepository.AddAsync(userDepot);
        await _applicationUserDepotRepository.SaveChangesAsync();
    }

    public async Task<UserResponse> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(request.Role))
        {
            ValidateRole(request.Role);
        }

        var applicationUserToUpdate = await _applicationUserRepository.GetByIdAsync(request.Id, cancellationToken);

        if (applicationUserToUpdate == null)
            throw new NotFoundException(nameof(ApplicationUser), request.Id);

        _mapper.Map(request, applicationUserToUpdate);
        _applicationUserRepository.Update(applicationUserToUpdate);
        await _applicationUserRepository.SaveChangesAsync(cancellationToken);

        var infrastructureUserToUpdate = await _userManager.Users.FirstOrDefaultAsync(iu => iu.ApplicationUserId == request.Id, cancellationToken: cancellationToken);

        if (infrastructureUserToUpdate == null)
            throw new NotFoundException(nameof(InfrastructureUser), request.Id);

        _mapper.Map(request, applicationUserToUpdate);
        await _userManager.UpdateAsync(infrastructureUserToUpdate);

        if (!string.IsNullOrEmpty(request.Role))
        {
            await UpdateUserRolesAsync(infrastructureUserToUpdate, request.Role);
        }

        var roles = await _userManager.GetRolesAsync(infrastructureUserToUpdate);

        var result = _mapper.Map<UserResponse>(applicationUserToUpdate);
        result.Roles = roles.ToList();

        return result;
    }

    public async Task DeleteUserFromDepotAsync(DeleteUserFromDepotRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetApplicationUserDepotSpecification(request.DepotId, request.UserId);
        
        var applicationUserDepotToRemove = await _applicationUserDepotRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);

        if (applicationUserDepotToRemove is null)
            throw new NotFoundException($"User with id {request.UserId} not found in depot with id {request.DepotId}");

        _applicationUserDepotRepository.Remove(applicationUserDepotToRemove);
        await _applicationUserDepotRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetUserDepotsAccesses(Guid userId, CancellationToken cancellationToken)
    {
        var specification = new GetUserDepotsSpecification(userId);

        var userDepots = await _applicationUserDepotRepository.GetAsync(specification, cancellationToken: cancellationToken);
        
        return userDepots.Select(ud => ud.DepotId).ToList();
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var applicationUserToRemove = await _applicationUserRepository.GetByIdAsync(id, cancellationToken);

        if (applicationUserToRemove is null)
            throw new NotFoundException(nameof(ApplicationUser), id);

        var infrastructureUserToRemove = await _userManager.Users
            .FirstOrDefaultAsync(u => u.ApplicationUserId == id, cancellationToken);

        if (infrastructureUserToRemove is null)
            throw new NotFoundException(nameof(InfrastructureUser), id);

        await _userManager.DeleteAsync(infrastructureUserToRemove);

        _applicationUserRepository.Remove(applicationUserToRemove);
        await _applicationUserRepository.SaveChangesAsync(cancellationToken);
    }

    private void ValidateRole(string role)
    {
        var validRoles = new[]
        {
            CustomRoles.SuperAdministrator,
            CustomRoles.Administrator,
            CustomRoles.Employee,
            CustomRoles.Driver
        };

        if (!validRoles.Contains(role))
        {
            throw new ArgumentException($"Invalid role: {role}");
        }
    }

    private async Task UpdateUserRolesAsync(InfrastructureUser user, string newRole)
    {
        var newRoles = new List<string>() { newRole };

        if (newRoles.Contains(CustomRoles.Administrator))
        {
            if (!newRoles.Contains(CustomRoles.Employee))
            {
                newRoles.Add(CustomRoles.Employee);
            }
        }

        if (newRoles.Contains(CustomRoles.SuperAdministrator))
        {
            if (!newRoles.Contains(CustomRoles.Administrator))
            {
                newRoles.Add(CustomRoles.Administrator);
            }
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in userRoles)
        {
            if (!newRoles.Contains(role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
        }

        foreach (var role in newRoles)
        {
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}