using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Responses;

public record HeartbeatResponse(
    [property: JsonProperty("currentTime", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    DateTimeOffset CurrentTime);
