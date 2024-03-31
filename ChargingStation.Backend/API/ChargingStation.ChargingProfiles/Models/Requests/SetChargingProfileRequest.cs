namespace ChargingStation.ChargingProfiles.Models.Requests;

public class SetChargingProfileRequest
{
    public Guid ChargingProfileId { get; set; }
    public Guid ConnectorId { get; set; }
    public Guid? TransactionId { get; set; }
}