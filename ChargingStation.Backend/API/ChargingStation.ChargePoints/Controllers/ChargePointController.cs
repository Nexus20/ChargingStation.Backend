﻿using ChargingStation.ChargePoints.Models.Requests;
using ChargingStation.ChargePoints.Models.Responses;
using ChargingStation.ChargePoints.Services;
using ChargingStation.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.ChargePoints.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChargePointController : ControllerBase
{
    private readonly IChargePointService _chargePointService;

    public ChargePointController(IChargePointService chargePointService)
    {
        _chargePointService = chargePointService;
    }

    [HttpPost("getall")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<ChargePointResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] GetChargePointRequest request, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _chargePointService.GetAsync(request, cancellationToken);

        return Ok(chargePoint);
    }

    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ChargePointResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _chargePointService.GetByIdAsync(id, cancellationToken);

        return Ok(chargePoint);
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ChargePointResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromForm] CreateChargePointRequest chargePoint, CancellationToken cancellationToken = default)
    {
        var createdChargePoint = await _chargePointService.CreateAsync(chargePoint, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, createdChargePoint);
    }
    
    [HttpPost("reset")]
    [Produces("application/json")]
    public async Task<IActionResult> ResetAsync([FromBody] ResetChargePointRequest request, CancellationToken cancellationToken = default)
    {
        await _chargePointService.ResetAsync(request, cancellationToken);

        return NoContent();
    }

    [HttpPut("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ChargePointResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromBody] UpdateChargePointRequest chargePoint, CancellationToken cancellationToken = default)
    {
        var updatedChargePoint = await _chargePointService.UpdateAsync(chargePoint, cancellationToken);

        return Ok(updatedChargePoint);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await _chargePointService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}