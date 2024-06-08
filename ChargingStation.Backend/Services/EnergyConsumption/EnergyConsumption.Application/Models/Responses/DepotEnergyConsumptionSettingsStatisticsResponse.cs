using ChargingStation.Common.Models.DepotEnergyConsumption.Dtos;

namespace EnergyConsumption.Application.Models.Responses;

public class DepotEnergyConsumptionSettingsStatisticsResponse
{
    public required List<ChargePointEnergyConsumptionSettingsStatisticsDto> ChargePointsLimits { get; set; }
    
    public required List<EnergyConsumptionIntervalSettingsDto> Intervals { get; set; }
}

public class ChargePointEnergyConsumptionSettingsStatisticsDto
{
    public Guid ChargePointId { get; set; }
    public double ChargePointEnergyLimit { get; set; }
    public string? ChargePointName { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}