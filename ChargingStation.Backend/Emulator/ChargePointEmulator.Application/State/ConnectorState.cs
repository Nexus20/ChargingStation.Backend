using System.Collections.Concurrent;
using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Messages_OCPP16.Requests.Enums;

namespace ChargePointEmulator.Application.State;

public class ConnectorState
{
    public int ConnectorId { get; set; }
    public StatusNotificationRequestStatus Status { get; set; }
    
    public TransactionState? LastTransaction { get; set; }
    
    public ConcurrentDictionary<int, CsChargingProfiles> ChargingProfiles { get; set; } = new();
}