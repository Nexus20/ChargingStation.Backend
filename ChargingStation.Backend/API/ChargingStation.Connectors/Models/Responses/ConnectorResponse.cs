using ChargingStation.Domain.Abstract;

namespace ChargingStation.Connectors.Models.Responses;

public class ConnectorResponse : ITimeMarkable
{
    public Guid Id { get; set; }
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ConnectorStatusResponse : ITimeMarkable
{
    public Guid Id { get; set; }
    public Guid ConnectorId { get; set; }
    public string? CurrentStatus { get; set; }
    public DateTime? StatusUpdatedTimestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ErrorCode { get; set; }
    public string? Info { get; set; }
    public string? VendorErrorCode { get; set; }
    public string? VendorId { get; set; }
}