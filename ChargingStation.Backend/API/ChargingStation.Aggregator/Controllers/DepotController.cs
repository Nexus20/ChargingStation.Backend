using ChargingStation.Aggregator.Models.Requests;
using ChargingStation.Aggregator.Models.Responses;
using ChargingStation.Aggregator.Services;
using ChargingStation.Common.Models.General;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.Aggregator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepotController : ControllerBase
{
    private readonly IDepotsAggregatorService _depotsAggregatorService;
    
    public DepotController(IDepotsAggregatorService depotsAggregatorService)
    {
        _depotsAggregatorService = depotsAggregatorService;
    }
    
    [HttpPost("getall")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<DepotAggregatedResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody]GetDepotsRequest request, CancellationToken cancellationToken = default)
    {
        var depots = await _depotsAggregatorService.GetAsync(request, cancellationToken);

        return Ok(depots);
    }
}