using ChargingStation.Common.Models.General;
using ChargingStation.Common.Rbac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.Application.Models.EnergyConsumption.Requests;
using Transactions.Application.Models.EnergyConsumption.Responses;
using Transactions.Application.Services.EnergyConsumption;

namespace Transactions.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EnergyConsumptionController : ControllerBase
{
    private readonly IEnergyConsumptionService _energyConsumptionService;

    public EnergyConsumptionController(IEnergyConsumptionService energyConsumptionService)
    {
        _energyConsumptionService = energyConsumptionService;
    }

    [HttpPost("chargepoints-consumption")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}, {CustomRoles.Employee}")]
    [ProducesResponseType(typeof(IPagedCollection<ChargePointsEnergyConsumptionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody]GetChargePointsEnergyConsumptionByDepotRequest request, CancellationToken cancellationToken = default)
    {
        var energyConsumptions = await _energyConsumptionService.GetChargePointsEnergyConsumptionByDepotAsync(request, cancellationToken);

        return Ok(energyConsumptions);
    }
}