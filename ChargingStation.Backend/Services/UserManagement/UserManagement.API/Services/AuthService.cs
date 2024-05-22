using ChargingStation.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using UserManagement.API.Models.Requests;
using UserManagement.API.Persistence;
using UserManagement.API.Models.Response;
using ChargingStation.Common.Exceptions;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using AutoMapper;

namespace UserManagement.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<InfrastructureUser> _userManager;
    private readonly IRepository<ApplicationUser> _applicationUserRepository;
    private readonly JwtHandler _jwtHandler;
    private readonly IMapper _mapper;

    public AuthService(UserManager<InfrastructureUser> userManager, IRepository<ApplicationUser> applicationUserRepository, 
        JwtHandler jwtHandler, IMapper mapper)
    {
        _userManager = userManager;
        _jwtHandler = jwtHandler;
        _applicationUserRepository = applicationUserRepository;
        _mapper = mapper;
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
}