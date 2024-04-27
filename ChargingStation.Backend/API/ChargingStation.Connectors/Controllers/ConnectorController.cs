using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.Connectors.Models.Requests;
using ChargingStation.Connectors.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.Connectors.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConnectorController : ControllerBase
{
    private readonly IConnectorService _connectorService;

    public ConnectorController(IConnectorService connectorService)
    {
        _connectorService = connectorService;
    }

    [HttpGet("{id:Guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ConnectorResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var connector = await _connectorService.GetByIdAsync(id, cancellationToken);
        return Ok(connector);
    }
    
    [HttpPost("GetOrCreate")]
    public async Task<IActionResult> GetOrCreateAsync([FromBody]GetOrCreateConnectorRequest request, CancellationToken cancellationToken)
    {
        var connector = await _connectorService.GetOrCreateConnectorAsync(request, cancellationToken);
            
        return Ok(connector);
    }
    
    [HttpPost("GetByChargePoints")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<ConnectorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByChargePointsIdsAsync([FromBody]List<Guid> chargePointsIds, CancellationToken cancellationToken)
    {
        var connectors = await _connectorService.GetByChargePointsIdsAsync(chargePointsIds, cancellationToken);
        
        return Ok(connectors);
    }
    
    [HttpPost("UpdateStatus")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateStatusAsync([FromBody]UpdateConnectorStatusRequest request, CancellationToken cancellationToken)
    {
        await _connectorService.UpdateConnectorStatusAsync(request, cancellationToken);
        
        return NoContent();
    }
    
    [HttpPost("changeavailability")]
    public async Task<IActionResult> ChangeAvailabilityAsync([FromBody] ChangeConnectorAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        await _connectorService.ChangeAvailabilityAsync(request, cancellationToken);

        return Accepted();
    }
}