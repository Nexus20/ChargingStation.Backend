using ChargingStation.Common.Models.DepotEnergyConsumption;
using ChargingStation.Common.Models.Depots.Responses;

namespace ChargingStation.Aggregator.Models.Responses;

public class DepotAggregatedDetailsResponse : DepotResponse
{
    public DepotEnergyConsumptionSettingsResponse? EnergyConsumptionSettings { get; set; }
}