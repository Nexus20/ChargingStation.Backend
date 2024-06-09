using Aggregator.Models.Responses;
using Aggregator.Services.Connectors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[ApiController]
[Authorize]
[Route("api/aggregator/[controller]")]
public class ConnectorController : ControllerBase
{
    private readonly IConnectorsAggregatorService _connectorsAggregatorService;

    public ConnectorController(IConnectorsAggregatorService connectorsAggregatorService)
    {
        _connectorsAggregatorService = connectorsAggregatorService;
    }
    
    [HttpPost("GetByChargePoints")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<ConnectorAggregatedResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByChargePointsIdsAsync([FromBody]List<Guid> chargePointsIds, CancellationToken cancellationToken)
    {
        var connectors = await _connectorsAggregatorService.GetByChargePointsIdsAsync(chargePointsIds, cancellationToken);
        
        return Ok(connectors);
    }
}