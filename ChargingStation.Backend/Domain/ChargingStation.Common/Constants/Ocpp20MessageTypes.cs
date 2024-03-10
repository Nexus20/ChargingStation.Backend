namespace ChargingStation.Common.Constants;

public static class Ocpp20MessageTypes
{
    public const string BootNotification = nameof(BootNotification);
    public const string Authorize = nameof(Authorize);
    public const string Heartbeat = nameof(Heartbeat);
    public const string StartTransaction = nameof(StartTransaction);
    public const string StopTransaction = nameof(StopTransaction);
    public const string MeterValues = nameof(MeterValues);
    public const string StatusNotification = nameof(StatusNotification);
    public const string DataTransfer = nameof(DataTransfer);
    public const string DiagnosticsStatusNotification = nameof(DiagnosticsStatusNotification);
}