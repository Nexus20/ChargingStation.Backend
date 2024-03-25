using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class ChargingSchedulePeriod : Entity
{
    public Guid ChargingProfileId { get; set; }
    public ChargingProfile ChargingProfile { get; set; }
    
    public int StartPeriod { get; set; }
    public double Limit { get; set; }
    public int NumberPhases { get; set; }
}