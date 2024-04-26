namespace ChargingStation.Common.Models.General;

public class SignalRMessage
{
    public string Payload { get; set; }
    public string PayloadType { get; set; }

    public SignalRMessage(string payload, string payloadType)
    {
        Payload = payload;
        PayloadType = payloadType;
    }
}

