namespace ChargingStation.Common.Constants;

public static class Ocpp16ActionTypes
{
    public const string BootNotification = nameof(BootNotification);
    public const string Authorize = nameof(Authorize);
    public const string Heartbeat = nameof(Heartbeat);
    public const string StartTransaction = nameof(StartTransaction);
    public const string StopTransaction = nameof(StopTransaction);
    public const string MeterValues = nameof(MeterValues);
    public const string StatusNotification = nameof(StatusNotification);
    public const string DataTransfer = nameof(DataTransfer);
    public const string Reset = nameof(Reset);
    public const string ReserveNow = nameof(ReserveNow);
    public const string CancelReservation = nameof(CancelReservation);
    public const string ChangeAvailability = nameof(ChangeAvailability);
    public const string SetChargingProfile = nameof(SetChargingProfile);
    public const string ClearChargingProfile = nameof(ClearChargingProfile);
}