using Aggregator.Models.Responses;
using Aggregator.Services;
using Aggregator.Services.Depots;
using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Rbac;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<DepotAggregatedResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody]GetDepotsRequest request, CancellationToken cancellationToken = default)
    {
        var depots = await _depotsAggregatorService.GetAsync(request, cancellationToken);

        return Ok(depots);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}, {CustomRoles.Employee}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DepotAggregatedDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var depot = await _depotsAggregatorService.GetByIdAsync(id, cancellationToken);

        return Ok(depot);
    }
}