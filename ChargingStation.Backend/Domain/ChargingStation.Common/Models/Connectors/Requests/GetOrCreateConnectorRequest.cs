namespace ChargingStation.Common.Models.Connectors.Requests;

public class GetOrCreateConnectorRequest
{
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
}