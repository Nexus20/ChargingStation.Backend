namespace ChargingStation.SignalR.Constants;

public static class HubMessageTypes
{
    public const string StationConnection = nameof(StationConnection);
    public const string ConnectorChanges = nameof(ConnectorChanges);
    public const string EnergyLimitExceeded = nameof(EnergyLimitExceeded);
    public const string Transaction = nameof(Transaction);
}