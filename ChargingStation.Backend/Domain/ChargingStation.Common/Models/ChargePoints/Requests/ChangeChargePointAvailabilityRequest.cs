using ChargingStation.Common.Messages_OCPP16.Requests.Enums;

namespace ChargingStation.Common.Models.ChargePoints.Requests;

public class ChangeChargePointAvailabilityRequest
{
    public required Guid ChargePointId { get; set; }
    public required ChangeAvailabilityRequestType AvailabilityType { get; set; }
}