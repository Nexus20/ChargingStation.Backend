using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class ChargingProfile : Entity, ITimeMarkable
{
    public int ChargingProfileId { get; set; }

    public int StackLevel { get; set; }
    
    public DateTime ValidFrom { get; set; }
    
    public DateTime ValidTo { get; set; }
    
    public ChargingProfileRecurrencyKind RecurrencyKind { get; set; }
    public ChargingProfilePurpose ChargingProfilePurpose { get; set; }
    public ChargingProfileKind ChargingProfileKind { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public int Duration { get; set; }
    public DateTime StartSchedule { get; set; }
    public ChargingScheduleChargingRateUnit SchedulingUnit { get; set; }
    public decimal MinChargingRate { get; set; }
    public List<ChargingSchedulePeriod> ChargingSchedulePeriods { get; set; }
    
    public List<ConnectorChargingProfile>? ConnectorChargingProfiles { get; set; }
}