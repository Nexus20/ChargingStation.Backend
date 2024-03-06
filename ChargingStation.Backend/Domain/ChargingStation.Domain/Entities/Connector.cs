using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class Connector : Entity, ITimeMarkable
{
    public required Guid ChargePointId { get; set; }
    public ChargePoint? ChargePoint { get; set; }
    public int ConnectorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<ConnectorStatus>? ConnectorStatuses { get; set; }
    public List<ConnectorMeterValue>? ConnectorMeterValues { get; set; }
}