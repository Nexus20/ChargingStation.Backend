using AutoMapper;
using ChargingStation.Depots.Models.Responses;
using ChargingStation.Depots.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.Depots.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepotController : ControllerBase
    {
        private readonly IDepotService _depotService;
        private readonly IMapper _mapper;

        public DepotController(IDepotService depotService, IMapper mapper)
        {
            _depotService = depotService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepotResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var depots = await _depotService.GetAsync(cancellationToken);

            return Ok(depots);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepotResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
        {
            var depot = await _depotService.GetByIdAsync(Guid.Parse(id), cancellationToken);

            return Ok(depot);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DepotResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromForm] DepotResponse depot, CancellationToken cancellationToken = default)
        {
            var createdDepot = await _depotService.CreateAsync(depot, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, createdDepot);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DepotResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromBody] DepotResponse depot, CancellationToken cancellationToken = default)
        {
            var updatedDepot = await _depotService.UpdateAsync(depot, cancellationToken);

            return Ok(updatedDepot);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
        {
            await _depotService.DeleteAsync(Guid.Parse(id), cancellationToken);

            return NoContent();
        }
    }
}
