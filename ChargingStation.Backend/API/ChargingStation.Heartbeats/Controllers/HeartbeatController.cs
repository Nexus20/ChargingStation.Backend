using ChargingStation.Heartbeats.Models;
using ChargingStation.Heartbeats.Models.Request;
using ChargingStation.Heartbeats.Services.Heartbeats;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.Heartbeats.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HeartbeatController : ControllerBase
{
    private readonly IHeartbeatService _heartbeatService;

    public HeartbeatController(IHeartbeatService heartbeatService)
    {
        _heartbeatService = heartbeatService;
    }

    [HttpPost("GetAll")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<HeartbeatEntity>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var heartbeats = await _heartbeatService.GetAsync(cancellationToken);

        return Ok(heartbeats);
    }

    [HttpPost("GetById")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(HeartbeatEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromBody] GetHeartbeatRequest request, CancellationToken cancellationToken)
    {
        var heartbeat = await _heartbeatService.GetByIdAsync(request, cancellationToken);

        return Ok(heartbeat);
    }
}