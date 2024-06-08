using ChargingStation.Common.Models.DepotEnergyConsumption;
using ChargingStation.Common.Rbac;
using EnergyConsumption.Application.Models.Requests;
using EnergyConsumption.Application.Models.Responses;
using EnergyConsumption.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnergyConsumption.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EnergyConsumptionSettingsController : ControllerBase
{
    private readonly IEnergyConsumptionSettingsService _energyConsumptionSettingsService;

    public EnergyConsumptionSettingsController(IEnergyConsumptionSettingsService energyConsumptionSettingsService)
    {
        _energyConsumptionSettingsService = energyConsumptionSettingsService;
    }
    
    [HttpPost("set")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}")]
    [Produces("text/plain")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> SetEnergyConsumptionSettingsAsync([FromBody] SetDepotEnergyConsumptionSettingsRequest request, CancellationToken cancellationToken = default)
    {
        var createdSettingsId = await _energyConsumptionSettingsService.SetEnergyConsumptionSettingsAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, createdSettingsId);
    }
    
    [HttpGet("{id}")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}, {CustomRoles.Employee}")]
    [ProducesResponseType(typeof(DepotEnergyConsumptionSettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var settings = await _energyConsumptionSettingsService.GetByIdAsync(id, cancellationToken);
        return Ok(settings);
    }
    
    [HttpGet("getByDepot/{depotId}")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}, {CustomRoles.Employee}")]
    [ProducesResponseType(typeof(DepotEnergyConsumptionSettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetByDepotAsync(Guid depotId, CancellationToken cancellationToken)
    {
        var settings = await _energyConsumptionSettingsService.GetByDepotIdAsync(depotId, cancellationToken);
        
        if(settings is null)
            return NoContent();
        
        return Ok(settings);
    }
    
    [HttpGet("getByChargingStation/{chargingStationId}")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}, {CustomRoles.Employee}")]
    [ProducesResponseType(typeof(DepotEnergyConsumptionSettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetByChargingStationAsync(Guid chargingStationId, CancellationToken cancellationToken)
    {
        var settings = await _energyConsumptionSettingsService.GetByChargingStationIdAsync(chargingStationId, cancellationToken);
        
        if(settings is null)
            return NoContent();
        
        return Ok(settings);
    }
    
    [HttpPost("statistics")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}, {CustomRoles.Employee}")]
    [ProducesResponseType(typeof(DepotEnergyConsumptionSettingsStatisticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetDepotEnergyConsumptionStatisticsAsync([FromBody] GetDepotEnergyConsumptionSettingsStatisticsRequest request, CancellationToken cancellationToken)
    {
        var statistics = await _energyConsumptionSettingsService.GetDepotEnergyConsumptionStatisticsAsync(request, cancellationToken);
        
        if(statistics is null)
            return NoContent();
        
        return Ok(statistics);
    }
}