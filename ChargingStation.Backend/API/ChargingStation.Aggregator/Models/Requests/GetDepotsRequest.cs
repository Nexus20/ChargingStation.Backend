using ChargingStation.Common.Models.General.Requests;
using ChargingStation.Domain.Enums;

namespace ChargingStation.Aggregator.Models.Requests;

public class GetDepotsRequest : BaseCollectionRequest
{
    public string? Name { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Building { get; set; }
    public DepotStatus? Status { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}