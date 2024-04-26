using ChargingStation.ChargingProfiles.Models.Requests.ConsumptionSettings;
using ChargingStation.ChargingProfiles.Services.EnergyConsumption;
using ChargingStation.Common.Models.DepotEnergyConsumption;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.ChargingProfiles.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnergyConsumptionSettingsController : ControllerBase
{
    private readonly IEnergyConsumptionSettingsService _energyConsumptionSettingsService;

    public EnergyConsumptionSettingsController(IEnergyConsumptionSettingsService energyConsumptionSettingsService)
    {
        _energyConsumptionSettingsService = energyConsumptionSettingsService;
    }
    
    [HttpPost("set")]
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
    [ProducesResponseType(typeof(DepotEnergyConsumptionSettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var settings = await _energyConsumptionSettingsService.GetByIdAsync(id, cancellationToken);
        return Ok(settings);
    }
    
    [HttpGet("getByDepot/{depotId}")]
    [Produces("application/json")]
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
    [ProducesResponseType(typeof(DepotEnergyConsumptionSettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetByChargingStationAsync(Guid chargingStationId, CancellationToken cancellationToken)
    {
        var settings = await _energyConsumptionSettingsService.GetByChargingStationIdAsync(chargingStationId, cancellationToken);
        
        if(settings is null)
            return NoContent();
        
        return Ok(settings);
    }
}