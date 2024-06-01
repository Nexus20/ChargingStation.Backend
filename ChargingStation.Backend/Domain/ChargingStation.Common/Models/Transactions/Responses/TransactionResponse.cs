using ChargingStation.Common.Models.Abstract;

namespace ChargingStation.Common.Models.Transactions.Responses;

public class TransactionResponse : BaseResponse, ITimeMarkable
{
    public int TransactionId { get; set; }
    public string StartTagId { get; set; }
    public DateTime StartTime { get; set; }
    public string? StopTagId { get; set; }
    public DateTime? StopTime { get; set; }
    public string? StopReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid ConnectorId { get; set; }
    public Guid? ReservationId { get; set; }
}