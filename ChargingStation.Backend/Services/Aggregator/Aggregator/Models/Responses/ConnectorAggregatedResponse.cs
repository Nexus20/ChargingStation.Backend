using ChargingStation.Common.Models.Connectors.Responses;

namespace Aggregator.Models.Responses;

public class ConnectorAggregatedResponse : ConnectorResponse
{
    public ConnectorConsumptionResponse Consumption { get; set; } = new();
}