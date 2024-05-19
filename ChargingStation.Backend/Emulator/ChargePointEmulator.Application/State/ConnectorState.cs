using ChargingStation.Common.Messages_OCPP16.Requests.Enums;

namespace ChargePointEmulator.Application.State;

public class ConnectorState
{
    public int ConnectorId { get; set; }
    public StatusNotificationRequestStatus Status { get; set; }
    
    public TransactionState? LastTransaction { get; set; }
}