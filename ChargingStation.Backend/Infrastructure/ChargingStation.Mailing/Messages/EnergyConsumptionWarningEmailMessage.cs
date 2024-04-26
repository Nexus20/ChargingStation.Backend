namespace ChargingStation.Mailing.Messages;

public class EnergyConsumptionWarningEmailMessage : IEmailMessage
{
    private readonly Guid _depotId;
    private readonly Guid _chargePointId;
    private readonly DateTime _warningTimestamp;
    private readonly double _energyConsumption;
    private readonly double _energyConsumptionLimit;

    public EnergyConsumptionWarningEmailMessage(Guid depotId, Guid chargePointId, DateTime warningTimestamp, double energyConsumption, double energyConsumptionLimit)
    {
        _depotId = depotId;
        _chargePointId = chargePointId;
        _warningTimestamp = warningTimestamp;
        _energyConsumption = energyConsumption;
        _energyConsumptionLimit = energyConsumptionLimit;
    }

    public string Subject => "Energy consumption warning";

    public string GetTextPart()
    {
        return $"Your energy consumption is too high!\nDepot ID: {_depotId}\nCharge point ID: {_chargePointId}\nWarning timestamp: {_warningTimestamp}\nEnergy consumption: {_energyConsumption}\nEnergy consumption limit: {_energyConsumptionLimit}";
    }

    public string GetHtmlPart()
    {
        return $"<h3>Your energy consumption is too high!</h3><p>Depot ID: {_depotId}</p><p>Charge point ID: {_chargePointId}</p><p>Warning timestamp: {_warningTimestamp}</p><p>Energy consumption: {_energyConsumption}</p><p>Energy consumption limit: {_energyConsumptionLimit}</p>";
    }
}