using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.OcppTags.Responses;
using Microsoft.AspNetCore.Mvc;
using OcppTags.Application.Models.Requests;
using OcppTags.Application.Services;

namespace OcppTags.Api.Controllers;

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
    
    [HttpGet("GetByTagId/{ocppTagId}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OcppTagResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetByOcppTagId(string ocppTagId, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _ocppTagService.GetByOcppTagIdAsync(ocppTagId, cancellationToken);
        
        if (chargePoint is null)
            return NoContent();

        return Ok(chargePoint);
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OcppTagResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateOcppTagRequest chargePoint, CancellationToken cancellationToken = default)
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