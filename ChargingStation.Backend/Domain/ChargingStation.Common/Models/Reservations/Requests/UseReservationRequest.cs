namespace ChargingStation.Common.Models.Reservations.Requests;

public class UseReservationRequest
{
    public Guid ChargePointId {get; set;}
    public Guid ConnectorId {get; set;}
    public int ReservationId {get; set;}
}
