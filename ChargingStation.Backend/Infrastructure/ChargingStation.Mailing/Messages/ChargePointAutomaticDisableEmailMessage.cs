namespace ChargingStation.Mailing.Messages;

public class ChargePointAutomaticDisableEmailMessage : IEmailMessage
{
    private readonly Guid _depotId;
    private readonly Guid _chargePointId;

    public ChargePointAutomaticDisableEmailMessage(Guid depotId, Guid chargePointId)
    {
        _depotId = depotId;
        _chargePointId = chargePointId;
    }

    public string Subject => "Charge point automatic disable";
    public string GetTextPart()
    {
        return $"Your charge point has been automatically disabled due to exceeding the power consumption limit!\nDepot ID: {_depotId}\nCharge point ID: {_chargePointId}";
    }

    public string GetHtmlPart()
    {
        return $"<h3>Your charge point has been automatically disabled due to exceeding the power consumption limit!</h3><p>Depot ID: {_depotId}</p><p>Charge point ID: {_chargePointId}</p>";
    }
}