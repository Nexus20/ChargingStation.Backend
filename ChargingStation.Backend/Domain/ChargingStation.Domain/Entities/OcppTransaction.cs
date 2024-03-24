using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class OcppTransaction : Entity, ITimeMarkable
{
    public int TransactionId { get; set; }
    public string Uid { get; set; } 
    public string StartTagId { get; set; }
    public DateTime StartTime { get; set; }
    public double MeterStart { get; set; }
    public string StartResult { get; set; }
    public string StopTagId { get; set; }
    public DateTime? StopTime { get; set; }
    public double? MeterStop { get; set; }
    public string? StopReason { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    
    public required Guid ConnectorId { get; set; }
    public Connector? Connector { get; set; }
    
    public List<ConnectorMeterValue>? ConnectorMeterValues { get; set; }
    public Reservation? Reservation { get; set; }
}