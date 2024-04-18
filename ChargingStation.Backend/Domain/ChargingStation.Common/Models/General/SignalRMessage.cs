
namespace ChargingStation.Common.Models.General;

public class SignalRMessage
{
    public Guid ChargePointId { get; set; }
    public string Payload { get; set; }
    public string PayloadType { get; set; }

    public SignalRMessage(Guid chargePointId, string payload, string payloadType)
    {
        ChargePointId = chargePointId;
        Payload = payload;
        PayloadType = payloadType;
    }
}

