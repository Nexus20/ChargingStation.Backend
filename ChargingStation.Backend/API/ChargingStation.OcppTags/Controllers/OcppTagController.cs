using ChargingStation.Common.Models;
using ChargingStation.OcppTags.Models.Requests;
using ChargingStation.OcppTags.Models.Responses;
using ChargingStation.OcppTags.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.OcppTags.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OcppTagController : ControllerBase
{
    private readonly IOcppTagService _ocppTagService;

    public OcppTagController(IOcppTagService ocppTagService)
    {
        _ocppTagService = ocppTagService;
    }

    [HttpPost("getall")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<OcppTagResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] GetOcppTagsRequest request, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _ocppTagService.GetAsync(request, cancellationToken);

        return Ok(chargePoint);
    }

    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OcppTagResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _ocppTagService.GetByIdAsync(id, cancellationToken);

        return Ok(chargePoint);
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OcppTagResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromForm] CreateOcppTagRequest chargePoint, CancellationToken cancellationToken = default)
    {
        var createdChargePoint = await _ocppTagService.CreateAsync(chargePoint, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, createdChargePoint);
    }

    [HttpPut("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OcppTagResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromBody] UpdateOcppTagRequest chargePoint, CancellationToken cancellationToken = default)
    {
        var updatedChargePoint = await _ocppTagService.UpdateAsync(chargePoint, cancellationToken);

        return Ok(updatedChargePoint);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await _ocppTagService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}