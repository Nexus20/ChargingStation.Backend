using Aggregator.Models.Responses;
using Aggregator.Services;
using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.General;
using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[ApiController]
[Route("api/aggregator/[controller]")]
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
    
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DepotAggregatedDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var depot = await _depotsAggregatorService.GetByIdAsync(id, cancellationToken);

        return Ok(depot);
    }
}