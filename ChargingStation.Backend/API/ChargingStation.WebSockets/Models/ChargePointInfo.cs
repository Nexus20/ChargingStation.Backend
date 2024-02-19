using System.Net.WebSockets;

namespace ChargingStation.WebSockets.Middlewares;

public class ChargePointInfo
{
    public Guid ChargePointId { get; set; }

    public Dictionary<string, string> RequestDictionary { get; set; } //Used for Central system initiated commands

    public Dictionary<string,object> ChargerResponse { get; set; } //Used for Central system initiated commands

    public WebSocket WebSocket { get; set; }

    public bool WebsocketBusy { get; set; }

    public bool Authorized { get; set; }

    public bool WaitingResponse => RequestDictionary.Count != 0;

    public ChargePointInfo(Guid chargePointId,WebSocket webSocket)
    {
        ChargePointId = chargePointId;
        WebSocket = webSocket;
        RequestDictionary = new Dictionary<string, string>();
        ChargerResponse = new Dictionary<string, object>();
        WebsocketBusy = false;
        Authorized=false;
    }
}