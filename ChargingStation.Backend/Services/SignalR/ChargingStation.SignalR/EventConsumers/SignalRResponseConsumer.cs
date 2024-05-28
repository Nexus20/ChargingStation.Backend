using ChargingStation.Common.Models.General;
using ChargingStation.InternalCommunication.SignalRModels;
using ChargingStation.SignalR.Constants;
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
                await HandleMessage<StationConnectionMessage>(message, HubMessageTypes.StationConnection);
                break;
            case nameof(ConnectorChangesMessage):
                await HandleMessage<ConnectorChangesMessage>(message, HubMessageTypes.ConnectorChanges);
                break;
            case nameof(TransactionMessage):
                await HandleMessage<TransactionMessage>(message, HubMessageTypes.Transaction);
                break;
            case nameof(EnergyLimitExceededMessage):
                await HandleMessage<EnergyLimitExceededMessage>(message, HubMessageTypes.EnergyLimitExceeded);
                break;
            case nameof(ChargePointAutomaticDisableMessage):
                await HandleMessage<ChargePointAutomaticDisableMessage>(message, HubMessageTypes.ChargePointAutomaticDisable);
                break;
            case nameof(ChargingProfileSetMessage):
                await HandleMessage<ChargingProfileSetMessage>(message, HubMessageTypes.ChargingProfileSet);
                break;
            case nameof(ChargingProfileClearedMessage):
                await HandleMessage<ChargingProfileClearedMessage>(message, HubMessageTypes.ChargingProfileCleared);
                break;
            case nameof(ReservationProcessedMessage):
                await HandleMessage<ReservationProcessedMessage>(message, HubMessageTypes.ReservationProcessed);
                break;
            case nameof(ReservationCancellationProcessedMessage):
                await HandleMessage<ReservationCancellationProcessedMessage>(message, HubMessageTypes.ReservationCancellationProcessed);
                break;
            case nameof(ChangeAvailabilityMessage):
                await HandleMessage<ChangeAvailabilityMessage>(message, HubMessageTypes.ChangeAvailability);
                break;
        }
    }

    private async Task HandleMessage<T>(SignalRMessage message, string hubMethod) where T : BaseMessage
    {
        var payload = JsonConvert.DeserializeObject<T>(message.Payload);
        await _hubFacade.SendCentralSystemMessageAsync(payload, hubMethod);
    }
}

