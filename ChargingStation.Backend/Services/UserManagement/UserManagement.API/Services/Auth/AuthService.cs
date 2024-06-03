using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Rbac;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Identity;
using ChargingStation.Infrastructure.Repositories;
using ChargingStation.Mailing.Messages;
using ChargingStation.Mailing.Services;
using Microsoft.AspNetCore.Identity;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;
using UserManagement.API.Utility;
using LoginRequest = UserManagement.API.Models.Requests.LoginRequest;
using RegisterRequest = UserManagement.API.Models.Requests.RegisterRequest;

namespace UserManagement.API.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<InfrastructureUser> _userManager;
    private readonly IRepository<ApplicationUser> _applicationUserRepository;
    private readonly JwtHandler _jwtHandler;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<InfrastructureUser> userManager, IRepository<ApplicationUser> applicationUserRepository,
        JwtHandler jwtHandler, IMapper mapper, IEmailService emailService, IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtHandler = jwtHandler;
        _applicationUserRepository = applicationUserRepository;
        _mapper = mapper;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var applicationUser = await _applicationUserRepository.GetByIdAsync(user.ApplicationUserId);

            var token = _jwtHandler.GenerateAuthToken(applicationUser!, userRoles.ToList(), DateTime.UtcNow.AddHours(1));

            return new TokenResponse() { Token = token };
        }

        throw new UnauthorizedException("Invalid credentials");
    }

    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        ValidateRole(registerRequest.Role);

        var applicationUser = _mapper.Map<ApplicationUser>(registerRequest);
        await _applicationUserRepository.AddAsync(applicationUser);

        var user = new InfrastructureUser
        {
            UserName = registerRequest.FirstName + applicationUser.LastName,
            Email = registerRequest.Email,
            ApplicationUserId = applicationUser.Id,
            PhoneNumber = registerRequest.Phone
        };
        var result = await _userManager.CreateAsync(user);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, registerRequest.Role);

            var roles = new List<string> { registerRequest.Role };
            await AssignAdditionalRolesAsync(user, registerRequest.Role, roles);

            var token = _jwtHandler.GenerateAuthToken(applicationUser, roles, DateTime.UtcNow.AddHours(1));
            
            var clientApplicationHost = _configuration["ClientApplicationHost"]!;
            var registrationLink = $"https://{clientApplicationHost}/auth/password-confirmation/?token={token}"; 
            var emailMessage = new RegistrationEmailMessage(registrationLink, registerRequest.FirstName);

            await _emailService.SendMessageAsync(emailMessage, registerRequest.Email);
        }
        else
        {
            throw new BadRequestException("Registration failed");
        }
    }

    public async Task ConfirmRegistration(ConfirmRegistrationRequest confirmRegistrationRequest)
    {
        var infrastructureUser = await _userManager.FindByEmailAsync(confirmRegistrationRequest.Email);

        if (infrastructureUser is null)
            throw new NotFoundException(nameof(InfrastructureUser), confirmRegistrationRequest.Email);

        await _userManager.AddPasswordAsync(infrastructureUser, confirmRegistrationRequest.Password);
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
            throw new BadRequestException($"Invalid role: {role}");
        }
    }

    private async Task AssignAdditionalRolesAsync(InfrastructureUser user, string role, List<string> roles)
    {
        if (role == CustomRoles.SuperAdministrator)
        {
            await _userManager.AddToRoleAsync(user, CustomRoles.Administrator);
            roles.Add(CustomRoles.Administrator);
        }
        else if (role == CustomRoles.Administrator)
        {
            await _userManager.AddToRoleAsync(user, CustomRoles.Employee);
            roles.Add(CustomRoles.Employee);
        }
    }
}