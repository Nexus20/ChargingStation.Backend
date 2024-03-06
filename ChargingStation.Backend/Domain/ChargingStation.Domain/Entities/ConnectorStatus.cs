using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class ConnectorStatus : Entity, ITimeMarkable
{
    public required Guid ConnectorId { get; set; }
    public Connector? Connector { get; set; }
    
    public string? CurrentStatus { get; set; }
    public DateTime? StatusUpdatedTimestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public string? ErrorCode { get; set; }
    public string? Info { get; set; }
    public string? VendorErrorCode { get; set; }
    public string? VendorId { get; set; }
}