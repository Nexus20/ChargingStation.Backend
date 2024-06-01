using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.TimeZone;
using ChargingStation.Common.Utility;
using Depots.Application.Models.Requests;
using Depots.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Depots.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepotController : ControllerBase
{
    private readonly IDepotService _depotService;

    public DepotController(IDepotService depotService)
    {
        _depotService = depotService;
    }

    [HttpPost("getall")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<DepotResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody]GetDepotsRequest request, CancellationToken cancellationToken = default)
    {
        var depots = await _depotService.GetAsync(request, cancellationToken);

        return Ok(depots);
    }

    [HttpGet("{id}")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}, {CustomRoles.Employee}")]
    [ProducesResponseType(typeof(DepotResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var depot = await _depotService.GetByIdAsync(id, cancellationToken);

        return Ok(depot);
    }

    [HttpPost]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}")]
    [ProducesResponseType(typeof(DepotResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateDepotRequest request, CancellationToken cancellationToken = default)
    {
        var createdDepot = await _depotService.CreateAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, createdDepot);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DepotResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromBody] UpdateDepotRequest request, CancellationToken cancellationToken = default)
    {
        var updatedDepot = await _depotService.UpdateAsync(request, cancellationToken);

        return Ok(updatedDepot);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await _depotService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    [HttpPost("listTimeZone")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<TimeZoneResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimeZone([FromBody] GetTimeZoneRequest request, CancellationToken cancellationToken = default)
    {
        var depots = await _depotService.GetTimeZonesAsync(request, cancellationToken);

        return Ok(depots);
    }
}