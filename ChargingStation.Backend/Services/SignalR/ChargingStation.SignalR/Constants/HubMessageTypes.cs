namespace ChargingStation.SignalR.Constants;

public static class HubMessageTypes
{
    public const string StationConnection = nameof(StationConnection);
    public const string ConnectorChanges = nameof(ConnectorChanges);
    public const string EnergyLimitExceeded = nameof(EnergyLimitExceeded);
    public const string Transaction = nameof(Transaction);
    public const string ChargePointAutomaticDisable = nameof(ChargePointAutomaticDisable);
    public const string ChargingProfileSet = nameof(ChargingProfileSet);
    public const string ChargingProfileCleared = nameof(ChargingProfileCleared);
    public const string ReservationProcessed = nameof(ChargingProfileCleared);
    public const string ReservationCancellationProcessed = nameof(ChargingProfileCleared);
    public const string ChangeAvailability = nameof(ChangeAvailability);
}