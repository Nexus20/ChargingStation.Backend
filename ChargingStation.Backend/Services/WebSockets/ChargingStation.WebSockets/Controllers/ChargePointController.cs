using AutoMapper;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.WebSockets.OcppConnectionHandlers;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.WebSockets.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChargePointController : ControllerBase
{
    private readonly IMapper _mapper;

    public ChargePointController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet("active")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<ActiveChargePointResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetActiveChargePoints()
    {
        var activeChargePoints = OcppWebSocketConnectionHandler.ActiveChargePoints;
        var response = _mapper.Map<List<ActiveChargePointResponse>>(activeChargePoints);
        return Ok(response);
    }
}