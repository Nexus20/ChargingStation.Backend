using Microsoft.AspNetCore.SignalR;

namespace ChargePointEmulator.UI.Hubs;

public static class HubMethods
{
    public const string ReceiveUpdateFromCentralSystem = nameof(ReceiveUpdateFromCentralSystem);
}

public class ChargePointHub : Hub
{
    public async Task HandleUpdateFromChargingStation()
    {
        await Clients.All.SendAsync(HubMethods.ReceiveUpdateFromCentralSystem);
    }
}