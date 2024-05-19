using ChargingStation.Common.Messages_OCPP16.Responses.Enums;

namespace ChargePointEmulator.Application.State;

public class ReservationState
{
    public int ConnectorId { get; set; }
    public int ReservationId { get; set; }
    public ReserveNowResponseStatus Status { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public string IdTag { get; set; }
}