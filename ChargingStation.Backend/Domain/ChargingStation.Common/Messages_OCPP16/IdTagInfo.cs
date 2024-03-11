using System.ComponentModel.DataAnnotations;
using ChargingStation.Common.Messages_OCPP16.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChargingStation.Common.Messages_OCPP16;

public record IdTagInfo
{
    [JsonProperty("status", Required = Required.Always)]
    [Required(AllowEmptyStrings = true)]
    [JsonConverter(typeof(StringEnumConverter))]
    public IdTagInfoStatus Status { get; set; }
    
    [JsonProperty("expiryDate", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? ExpiryDate { get; set; }

    [JsonProperty("parentIdTag", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    [StringLength(20)]
    public string? ParentIdTag { get; set; }
}
