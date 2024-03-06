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
    
    [HttpPost("UpdateStatus")]
    public async Task<IActionResult> UpdateStatusAsync([FromBody]UpdateConnectorStatusRequest request, CancellationToken cancellationToken)
    {
        await _connectorService.UpdateConnectorStatusAsync(request, cancellationToken);
        
        return NoContent();
    }
}