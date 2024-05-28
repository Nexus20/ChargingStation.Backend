using ChargingStation.Common.Messages_OCPP16.Requests.Enums;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;

namespace ChargingStation.InternalCommunication.SignalRModels;

public class ChargingProfileClearedMessage : BaseMessage
{
    public ClearChargingProfileResponseStatus Status { get; set; }
    
    public Guid? ChargingProfileId { get; init; }
    
    public int? ConnectorId { get; init; }

    public ClearChargingProfileRequestChargingProfilePurpose? ChargingProfilePurpose { get; init; }
    
    public int? StackLevel { get; init; }
}