using ChargingStation.Common.Models.Abstract;

namespace ChargingStation.Common.Models.Connectors.Responses;

public class ConnectorResponse : ITimeMarkable
{
    public Guid Id { get; set; }
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ConnectorStatusResponse? CurrentStatus { get; set; }
    public List<Guid>? ChargingProfilesIds { get; set; } 
}