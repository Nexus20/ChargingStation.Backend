using ChargingStation.Common.Models.General;
using ChargingStation.InternalCommunication.SignalRModels;
using ChargingStation.SignalR.Hubs;
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
            case nameof(StationConnectionMessage):
                await HandleMessage<StationConnectionMessage>(message, _hubFacade.SendBootNotification);
                break;
            case nameof(ConnectorChangesMessage):
                await HandleMessage<ConnectorChangesMessage>(message, _hubFacade.SendMeterValue);
                break;
            case nameof(TransactionMessage):
                await HandleMessage<TransactionMessage>(message, _hubFacade.SendStartTransaction);
                break;
            case nameof(EnergyLimitExceededMessage):
                await HandleMessage<EnergyLimitExceededMessage>(message, _hubFacade.SendEnergyLimitExceeded);
                break;
            default:
                break;
        }
    }

    private async Task HandleMessage<T>(SignalRMessage message, Func<T, Task> sendMethod) where T : BaseMessage
    {
        var payload = JsonConvert.DeserializeObject<T>(message.Payload);
        await sendMethod(payload);
    }
}

