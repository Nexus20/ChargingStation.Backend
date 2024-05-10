using ChargingStation.Common.Messages_OCPP16.Enums;

namespace ChargingStation.ChargingProfiles.Models.Requests;

public class CreateChargingProfileRequest
{
    public int StackLevel { get; set; }
    
    public DateTime ValidFrom { get; set; }
    
    public DateTime ValidTo { get; set; }
    
    public ChargingProfileRecurrencyKind RecurrencyKind { get; set; }
    public ChargingProfilePurpose ChargingProfilePurpose { get; set; }
    public ChargingProfileKind ChargingProfileKind { get; set; }
    
    public int Duration { get; set; }
    public DateTime StartSchedule { get; set; }
    public ChargingScheduleChargingRateUnit SchedulingUnit { get; set; }
    public decimal MinChargingRate { get; set; }
    
    public List<ChargingSchedulePeriodRequest> ChargingSchedulePeriods { get; set; }
}