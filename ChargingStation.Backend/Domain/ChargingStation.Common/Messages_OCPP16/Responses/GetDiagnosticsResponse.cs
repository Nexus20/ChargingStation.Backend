using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Responses;

public record GetDiagnosticsResponse
{
    [JsonProperty("fileName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    [StringLength(255)]
    public string? FileName { get; init; }
}
