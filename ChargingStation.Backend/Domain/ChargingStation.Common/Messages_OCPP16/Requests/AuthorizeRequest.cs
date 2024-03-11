using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ChargingStation.Common.Messages_OCPP16.Requests;

public record AuthorizeRequest(
    [property: JsonProperty("idTag", Required = Required.Always)]
    [property: Required(AllowEmptyStrings = true)]
    [property: StringLength(20)]
    string IdTag);
