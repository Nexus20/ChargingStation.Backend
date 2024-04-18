using System.Text;
using Newtonsoft.Json;

namespace ChargingStation.Common.Models.General;

public class IntegrationOcppMessage<TPayload>
{
    public IntegrationOcppMessage(Guid chargePointId, TPayload payload, string ocppMessageId, string ocppProtocol)
    {
        ChargePointId = chargePointId;
        Payload = payload;
        OcppMessageId = ocppMessageId;
        OcppProtocol = ocppProtocol;
    }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ChargePointId { get; private set; }
    public TPayload Payload { get; set; }
    public string OcppMessageId { get; set; }
    public string OcppProtocol { get; set; }
}

public sealed class CentralSystemRequestIntegrationOcppMessage : IntegrationOcppMessage<string>
{
    public string Action { get; set; }
    
    public CentralSystemRequestIntegrationOcppMessage(Guid chargePointId, string action, string payload, string ocppMessageId, string ocppProtocol) 
        : base(chargePointId, JsonConvert.SerializeObject(payload), ocppMessageId, ocppProtocol)
    {
        Action = action;
    }
    
    public static CentralSystemRequestIntegrationOcppMessage Create<TMessage>(Guid chargePointId, TMessage payload, string action, string ocppMessageId, string ocppProtocol)
    {
        var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
        return new CentralSystemRequestIntegrationOcppMessage(chargePointId, action, payloadBase64, ocppMessageId, ocppProtocol);
    }
}

public sealed class CentralSystemResponseIntegrationOcppMessage : IntegrationOcppMessage<string>
{
    public CentralSystemResponseIntegrationOcppMessage(Guid chargePointId, string payload, string ocppMessageId, string ocppProtocol)
        : base(chargePointId, JsonConvert.SerializeObject(payload), ocppMessageId, ocppProtocol)
    {
    }
    
    public static CentralSystemResponseIntegrationOcppMessage Create<TMessage>(Guid chargePointId, TMessage payload, string ocppMessageId, string ocppProtocol)
    {
        var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
        return new CentralSystemResponseIntegrationOcppMessage(chargePointId, payloadBase64, ocppMessageId, ocppProtocol);
    }
}