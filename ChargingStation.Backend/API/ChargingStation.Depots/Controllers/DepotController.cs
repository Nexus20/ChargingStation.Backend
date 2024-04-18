using ChargingStation.Common.Models;
using ChargingStation.Common.Models.General;
using ChargingStation.Depots.Models.Requests;
using ChargingStation.Depots.Models.Responses;
using ChargingStation.Depots.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.Depots.Controllers;

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
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<DepotResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody]GetDepotsRequest request, CancellationToken cancellationToken = default)
    {
        var depots = await _depotService.GetAsync(request, cancellationToken);

        return Ok(depots);
    }

    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DepotResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var depot = await _depotService.GetByIdAsync(id, cancellationToken);

        return Ok(depot);
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DepotResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateDepotRequest request, CancellationToken cancellationToken = default)
    {
        var createdDepot = await _depotService.CreateAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, createdDepot);
    }

    [HttpPut("{id}")]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await _depotService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}