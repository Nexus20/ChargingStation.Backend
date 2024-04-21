﻿using ChargingStation.InternalCommunication.SignalRModels;
using ChargingStation.SignalR.Constants;
using Microsoft.AspNetCore.SignalR;

namespace ChargingStation.SignalR.Hubs;

public class HubFacade
{
    private readonly IHubContext<ChargingStationHub> _hubContext;

    public HubFacade(IHubContext<ChargingStationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendBootNotification(StationConnectionMessage stationConnectionMessage)
    {
        await _hubContext.Clients.All.SendAsync(HubMessageTypes.StationConnection, stationConnectionMessage);
    }

    public async Task SendMeterValue(ConnectorChangesMessage connectorChangesMessage)
    {
        await _hubContext.Clients.All.SendAsync(HubMessageTypes.ConnectorChanges, connectorChangesMessage);
    }

    public async Task SendStartTransaction(TransactionMessage transactionMessage)
    {
        await _hubContext.Clients.All.SendAsync(HubMessageTypes.Transaction, transactionMessage);
    }
}