using ChargingStation.ChargingProfiles.Models.Requests;
using ChargingStation.ChargingProfiles.Models.Responses;
using ChargingStation.ChargingProfiles.Services;
using ChargingStation.Common.Models.General;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.ChargingProfiles.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChargingProfileController : ControllerBase
{
    private readonly IChargingProfileService _chargingProfileService;

    public ChargingProfileController(IChargingProfileService chargingProfileService)
    {
        _chargingProfileService = chargingProfileService;
    }
    
    [HttpPost("getall")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<ChargingProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] GetChargingProfilesRequest request, CancellationToken cancellationToken = default)
    {
        var reservation = await _chargingProfileService.GetAsync(request, cancellationToken);

        return Ok(reservation);
    }
    
    [HttpGet("{id:Guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ChargingProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _chargingProfileService.GetByIdAsync(id, cancellationToken);
        return Ok(reservation);
    }
    
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ChargingProfileResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateChargingProfileAsync([FromBody] CreateChargingProfileRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _chargingProfileService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    
    [HttpPost("set")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SetChargingProfileAsync([FromBody] SetChargingProfileRequest request, CancellationToken cancellationToken = default)
    {
        await _chargingProfileService.SetChargingProfileAsync(request, cancellationToken);
        return Accepted();
    }
    
    [HttpPost("clear")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> ClearChargingProfileAsync([FromBody] ClearChargingProfileRequest request, CancellationToken cancellationToken = default)
    {
        await _chargingProfileService.ClearChargingProfileAsync(request, cancellationToken);
        return Accepted();
    }
}