using ChargingStation.InternalCommunication.SignalRModels;
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
    
    public async Task SendCentralSystemMessageAsync(BaseMessage message, string messageType)
    {
        await _hubContext.Clients.All.SendAsync(messageType, message);
    }
}