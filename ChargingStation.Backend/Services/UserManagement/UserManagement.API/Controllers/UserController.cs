using System.Security.Claims;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Rbac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Models.Requests;
using UserManagement.API.Models.Response;
using UserManagement.API.Services.Users;

namespace UserManagement.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("getall")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}")]
    [ProducesResponseType(typeof(IPagedCollection<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] GetUsersRequest request, CancellationToken cancellationToken)
    {
        var users = await _userService.GetAsync(request, cancellationToken);

        return Ok(users);
    }
    
    [HttpGet("{id}")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);

        return Ok(user);
    }
    
    [HttpGet("profile")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPersonalInfo(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (userId == null || !Guid.TryParse(userId, out var userGuid))
            return BadRequest("Invalid token claims");
        
        var user = await _userService.GetPersonalInfoAsync(userGuid, cancellationToken);

        return Ok(user);
    }

    [HttpPost("invite")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Invite([FromBody] InviteRequest inviteRequest)
    {
        var invitationToken = _userService.GenerateInvitationToken(inviteRequest);
        var clientApplicationHost = _configuration["ClientApplicationHost"]!;

        var invitationLink = $"https://{clientApplicationHost}/auth/confirm-invite/?token={invitationToken}";

        await _userService.SendInvitationEmailAsync(inviteRequest, invitationLink);

        return NoContent();
    }

    [HttpGet("confirm-invite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmInvite([FromQuery] string token)
    {
        await _userService.ConfirmInvite(token);

        return NoContent();
    }

    [HttpDelete("deleteUserFromDepot")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserFromDepot([FromBody] DeleteUserFromDepotRequest request, CancellationToken cancellationToken)
    {
        await _userService.DeleteUserFromDepotAsync(request, cancellationToken);

        return NoContent();
    }

    [HttpPut]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromBody] UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var updatedUser = await _userService.UpdateAsync(request, cancellationToken);

        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        await _userService.DeleteUserAsync(id, cancellationToken);

        return NoContent();
    }
}