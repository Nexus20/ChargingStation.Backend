using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class ConnectorMeterValue : Entity, ITimeMarkable
{
    public required Guid TransactionId { get; set; }
    public OcppTransaction? Transaction { get; set; }
    
    public required Guid ConnectorId { get; set; }
    public Connector? Connector { get; set; }

    public required string Value { get; set; }
    public string? Format { get; set; }
    public string? Location { get; set; }
    public string? Measurand { get; set; }
    public string? Unit { get; set; }
    public string? Phase { get; set; }
    public DateTime? MeterValueTimestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}