namespace ChargingStation.Common.Models.Connectors.Requests;

public class UpdateConnectorStatusRequest
{
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
    public required string Status { get; set; }
    public DateTime StatusTimestamp { get; set; }
    public string? ErrorCode { get; set; }
    public string? Info { get; set; }
    public string? VendorErrorCode { get; set; }
    public string? VendorId { get; set; }
}