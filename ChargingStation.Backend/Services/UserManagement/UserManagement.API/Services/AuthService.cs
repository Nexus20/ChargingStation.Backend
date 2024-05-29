﻿using ChargingStation.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using UserManagement.API.Models.Requests;
using UserManagement.API.Persistence;
using UserManagement.API.Models.Response;
using ChargingStation.Common.Exceptions;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using AutoMapper;
using ChargingStation.Mailing.Messages;
using ChargingStation.Mailing.Services;
using System.Security.Claims;

namespace UserManagement.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<InfrastructureUser> _userManager;
    private readonly IRepository<ApplicationUser> _applicationUserRepository;
    private readonly IRepository<ApplicationUserDepot> _applicationUserDepotRepository;
    private readonly JwtHandler _jwtHandler;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public AuthService(UserManager<InfrastructureUser> userManager, IRepository<ApplicationUser> applicationUserRepository, 
        IRepository<ApplicationUserDepot> applicationUserDepotRepository,JwtHandler jwtHandler, IMapper mapper, 
        IEmailService emailService)
    {
        _userManager = userManager;
        _jwtHandler = jwtHandler;
        _applicationUserRepository = applicationUserRepository;
        _applicationUserDepotRepository = applicationUserDepotRepository;
        _mapper = mapper;
        _emailService = emailService;
        
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var token = _jwtHandler.GenerateToken(user, userRoles.FirstOrDefault(), DateTime.UtcNow.AddHours(1));

            return new TokenResponse() { Token = token };
        }

        throw new UnauthorizedException("Invalid credentials");
    }

    public async Task<TokenResponse> RegisterAsync(RegisterRequest registerRequest)
    {
        var applicationUser = _mapper.Map<ApplicationUser>(registerRequest);
        await _applicationUserRepository.AddAsync(applicationUser);

        var user = new InfrastructureUser
        {
            UserName = registerRequest.FirstName + applicationUser.LastName,
            Email = registerRequest.Email,
            ApplicationUserId = applicationUser.Id,
        };
        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, registerRequest.Role);

            var token = _jwtHandler.GenerateToken(user, registerRequest.Role, DateTime.UtcNow.AddHours(1));

            return new TokenResponse() { Token = token };
        }

        throw new BadRequestException("Registration failed");
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
            ApplicationUserId = Guid.Parse(user.Id),
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
}