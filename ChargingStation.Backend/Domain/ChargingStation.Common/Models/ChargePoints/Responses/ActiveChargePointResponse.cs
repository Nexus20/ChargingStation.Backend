using ChargingStation.Common.Models.General;

namespace ChargingStation.Common.Models.ChargePoints.Responses;

public class ActiveChargePointResponse
{
    public Guid ChargePointId { get; set; }
    public Dictionary<string, OcppMessage>? RequestDictionary { get; set; } //Used for Central system initiated commands

    public Dictionary<string,object>? ChargerResponse { get; set; } //Used for Central system initiated commands

    public bool WebsocketBusy { get; set; }

    public bool Authorized { get; set; }
    public bool WaitingForResponse { get; set; }
}