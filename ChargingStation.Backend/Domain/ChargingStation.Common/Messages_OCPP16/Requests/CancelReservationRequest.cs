using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record CancelReservationRequest(
    [property: JsonProperty("reservationId", Required = Required.Always)]
    int ReservationId);
