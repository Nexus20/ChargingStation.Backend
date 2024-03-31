using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class ConnectorChargingProfile : Entity, ITimeMarkable
{
    public Guid ConnectorId { get; set; }
    public Connector Connector { get; set; }
    
    public Guid ChargingProfileId { get; set; }
    public ChargingProfile ChargingProfile { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}