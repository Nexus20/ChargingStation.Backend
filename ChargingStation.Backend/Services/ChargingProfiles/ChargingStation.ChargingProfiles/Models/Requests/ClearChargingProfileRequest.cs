namespace ChargingStation.ChargingProfiles.Models.Requests;

public class ClearChargingProfileRequest
{
    public Guid ChargingProfileId { get; init; }
    public Guid ConnectorId { get; init; }
}