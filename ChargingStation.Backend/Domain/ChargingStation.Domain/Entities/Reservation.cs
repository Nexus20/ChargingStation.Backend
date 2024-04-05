using System.Diagnostics.CodeAnalysis;
using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class Reservation : Entity, ITimeMarkable
{
    public int ReservationId { get; set; }
    
    public required Guid ChargePointId { get; set; }
    [AllowNull]
    public ChargePoint ChargePoint { get; set; }
    
    public Guid? ConnectorId { get; set; }
    public Connector? Connector { get; set; }
    
    public Guid? TransactionId { get; set; }
    public OcppTransaction? Transaction { get; set; }
    
    public Guid TagId { get; set; }
    [AllowNull]
    public OcppTag Tag { get; set; }
    
    public string? Status { get; set; }
    
    public DateTime StartDateTime { get; set; }
    public DateTime ExpiryDateTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public required string ReservationRequestId { get; set; }
    public string? CancellationRequestId { get; set; }
    public bool IsCancelled { get; set; }
    public bool IsUsed { get; set; }
}