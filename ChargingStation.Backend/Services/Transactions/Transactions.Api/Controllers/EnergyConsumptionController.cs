using ChargingStation.Common.Models.ConnectorEnergyConsumption;
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
    
    [HttpPost("depot-consumption-statistics")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}, {CustomRoles.Employee}")]
    [ProducesResponseType(typeof(List<DepotEnergyConsumptionStatisticsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepotEnergyConsumption([FromBody]GetDepotEnergyConsumptionStatisticsRequest request, CancellationToken cancellationToken = default)
    {
        var energyConsumptions = await _energyConsumptionService.GetDepotEnergyConsumption(request, cancellationToken);

        return Ok(energyConsumptions);
    }
    
    [HttpPost("connectors-consumption")]
    [Produces("application/json")]
    [Authorize(Roles = $"{CustomRoles.SuperAdministrator}, {CustomRoles.Administrator}, {CustomRoles.Employee}")]
    [ProducesResponseType(typeof(List<ConnectorEnergyConsumptionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConnectorsEnergyConsumption([FromBody]List<Guid> connectorsIds, CancellationToken cancellationToken = default)
    {
        var energyConsumptions = await _energyConsumptionService.GetConnectorsEnergyConsumptionAsync(connectorsIds, cancellationToken);

        return Ok(energyConsumptions);
    }
}