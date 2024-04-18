using ChargingStation.Common.Models.General;
using ChargingStation.SignalR.Hubs;
using ChargingStation.SignalR.Models;
using MassTransit;
using Newtonsoft.Json;


namespace ChargingStation.SignalR.EventConsumers;

public class SignalRResponseConsumer : IConsumer<SignalRMessage>
{
    private readonly HubFacade _hubFacade;

    public SignalRResponseConsumer(HubFacade hubFacade)
    {
        _hubFacade = hubFacade;
    }

    public async Task Consume(ConsumeContext<SignalRMessage> context)
    {
        var message = context.Message;

        switch (message.PayloadType)
        {
            case "UpdateChargePointRequest":
                await HandleMessage<StationConnectionMessage>(message, _hubFacade.SendBootNotification);
                break;
            case "UpdateConnectorStatusRequest":
            case "ConnectorMeterValue":
                await HandleMessage<ConnectorChangesMessage>(message, _hubFacade.SendMeterValue);
                break;
            case "OcppTransaction":
                await HandleMessage<TransactionMessage>(message, _hubFacade.SendStartTransaction);
                break;
            default:
                break;
        }
    }

    private async Task HandleMessage<T>(SignalRMessage message, Func<T, Task> sendMethod) where T : BaseMassage
    {
        var payload = JsonConvert.DeserializeObject<T>(message.Payload);
        payload.ChargePointId = message.ChargePointId;
        await sendMethod(payload);
    }
}

