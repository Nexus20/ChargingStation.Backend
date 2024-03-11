using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Responses;

public record AuthorizeResponse
{
    [JsonProperty("idTagInfo", Required = Required.Always)]
    [Required]
    public IdTagInfo IdTagInfo { get; set; }
}