namespace ChargingStation.InternalCommunication.SignalRModels;

public class ChargePointAutomaticDisableMessage : BaseMessage
{
    public Guid DepotId {get; set; }
}