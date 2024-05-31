﻿using ChargingStation.Common.Models.General;
using ChargingStation.Common.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;
using UserManagement.API.Services;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [Produces("application/json")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var token = await _authService.LoginAsync(loginRequest);

        return Ok(token);
    }

    [HttpPost("register")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        await _authService.RegisterAsync(registerRequest);

        return NoContent();
    }

    [HttpPost("invite")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Invite([FromBody] InviteRequest inviteRequest)
    {
        var invitationToken = _authService.GenerateInvitationToken(inviteRequest);

        var invitationLink = Url.Action("ConfirmInvite", "Auth", new { token = invitationToken }, Request.Scheme);

        await _authService.SendInvitationEmailAsync(inviteRequest, invitationLink);

        return NoContent();
    }

    [HttpGet("confirm-invite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmInvite([FromQuery] string token)
    {
        await _authService.ConfirmInvite(token);

        return NoContent();
    }

    [HttpGet("confirm-registration")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmRegistration([FromBody] ConfirmRegistrationRequest confirmRegistrationRequest)
    {
        await _authService.ConfirmRegistration(confirmRegistrationRequest);

        return NoContent();
    }


    [HttpPost("getAllUsers")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromBody] GetUserRequest request, CancellationToken cancellationToken = default)
    {
        var users = await _authService.GetUsers(request, cancellationToken);

        return Ok(users);
    }
}
