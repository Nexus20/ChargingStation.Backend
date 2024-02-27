using System.Text;
using Newtonsoft.Json;

namespace ChargingStation.Common.Models;

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

public sealed class ResponseIntegrationOcppMessage : IntegrationOcppMessage<string>
{
    public ResponseIntegrationOcppMessage(Guid chargePointId, string payload, string ocppMessageId, string ocppProtocol) 
        : base(chargePointId, JsonConvert.SerializeObject(payload), ocppMessageId, ocppProtocol)
    {
    }
    
    public static ResponseIntegrationOcppMessage Create<TMessage>(Guid chargePointId, TMessage payload, string ocppMessageId, string ocppProtocol) where TMessage : class
    {
        var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
        return new ResponseIntegrationOcppMessage(chargePointId, payloadBase64, ocppMessageId, ocppProtocol);
    }
}