using ChargingStation.Common.Messages_OCPP16.Enums;

namespace ChargePoints.Application.Models.Requests;

public class ResetChargePointRequest
{
    public required Guid ChargePointId { get; set; }
    public ResetRequestType ResetType { get; set; } = ResetRequestType.Soft;
}