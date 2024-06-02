namespace Transactions.Application.Services.EnergyConsumption;

public interface IEnergyConsumptionService
{
    
}

public class GetChargePointsEnergyConsumptionByDepotRequest
{
    public required DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    
    public required Guid DepotId { get; set; }
}

public class ChargePointsEnergyConsumptionResponse
{
    public required DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    
    public required List<ChargePointEnergyConsumptionResponse> ChargePointsConsumption { get; set; }
}

public class ChargePointEnergyConsumptionResponse
{
    public required Guid ChargePointId { get; set; }
    public required string ChargePointName { get; set; }
    public required decimal EnergyConsumed { get; set; }
    
    public required List<ChargePointConnectorEnergyConsumptionResponse>
}

public class ChargePointConnectorEnergyConsumptionResponse
{
    public required Guid ConnectorId { get; set; }
    public required int ConnectorNumber { get; set; }
    public required decimal EnergyConsumed { get; set; }
}