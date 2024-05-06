namespace ChargingStation.Common.Models.Connectors.Requests;

public class GetConnectorByChargePointIdRequest
{
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
}