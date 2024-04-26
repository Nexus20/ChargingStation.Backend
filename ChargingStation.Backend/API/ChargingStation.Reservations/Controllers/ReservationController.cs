using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.Reservations.Requests;
using ChargingStation.Reservations.Models.Requests;
using ChargingStation.Reservations.Models.Responses;
using ChargingStation.Reservations.Services.Reservations;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.Reservations.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost("getall")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<ReservationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] GetReservationsRequest request, CancellationToken cancellationToken = default)
    {
        var reservations = await _reservationService.GetAsync(request, cancellationToken);
        return Ok(reservations);
    }
    
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationService.GetByIdAsync(id, cancellationToken);
        return Ok(reservation);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReservationAsync([FromBody] CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        await _reservationService.CreateReservationAsync(request, cancellationToken);
        return Created();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateReservationAsync([FromBody] UpdateReservationRequest request, CancellationToken cancellationToken = default)
    {
        await _reservationService.UpdateReservationAsync(request, cancellationToken);
        return NoContent();
    }
    
    [HttpPost("cancel")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelReservationAsync([FromBody] CreateReservationCancellationRequest request, CancellationToken cancellationToken = default)
    {
        await _reservationService.CreateReservationCancellation(request, cancellationToken);
        return Accepted();
    }
    
    [HttpPost("use")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UseReservationAsync([FromBody] UseReservationRequest request, CancellationToken cancellationToken = default)
    {
        await _reservationService.UseReservationAsync(request, cancellationToken);
        return NoContent();
    }
}