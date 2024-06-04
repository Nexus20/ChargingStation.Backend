using ChargingStation.Common.Rbac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;
using UserManagement.API.Services.Auth;

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

    [HttpGet("confirm-registration")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmRegistration([FromBody] ConfirmRegistrationRequest confirmRegistrationRequest)
    {
        await _authService.ConfirmRegistration(confirmRegistrationRequest);

        return NoContent();
    }
}
