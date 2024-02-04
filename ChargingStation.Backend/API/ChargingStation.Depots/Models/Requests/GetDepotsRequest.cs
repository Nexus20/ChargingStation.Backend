using ChargingStation.Common.Models.Requests;
using ChargingStation.Domain.Enums;

namespace ChargingStation.Depots.Models.Requests;

public class GetDepotsRequest : BaseCollectionRequest
{
    public string? Name { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Building { get; set; }
    public DepotStatus? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}