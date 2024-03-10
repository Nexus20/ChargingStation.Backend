using ChargingStation.Common.Messages_OCPP16;

namespace ChargingStation.ChargePoints.Models.Requests;

public class ResetChargePointRequest
{
    public required Guid ChargePointId { get; set; }
    public ResetRequestType ResetType { get; set; } = ResetRequestType.Soft;
}