﻿using System.Security.Claims;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Rbac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

    public UserController(IUserService userService)
    {
        _userService = userService;
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
}