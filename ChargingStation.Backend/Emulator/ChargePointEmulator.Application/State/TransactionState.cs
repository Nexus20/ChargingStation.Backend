using System.Collections.Concurrent;
using ChargingStation.Common.Messages_OCPP16.Requests;

namespace ChargePointEmulator.Application.State;

public class TransactionState
{
    public int TransactionId { get; set; }
    public DateTime StartTimestamp { get; set; }
    public ConcurrentBag<MeterValuesRequest> MeterValues { get; set; } = [];
    public DateTime? StopTimestamp { get; set; }
}