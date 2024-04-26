namespace ChargingStation.InternalCommunication.SignalRModels;

public class EnergyLimitExceededMessage : BaseMessage
{
    public Guid DepotId {get; set; }
    public DateTime WarningTimestamp {get; set; }
    public double EnergyConsumption {get; set; }
    public double EnergyConsumptionLimit {get; set; }
}