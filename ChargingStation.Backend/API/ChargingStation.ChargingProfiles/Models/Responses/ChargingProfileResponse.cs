using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Common.Models.Abstract;

namespace ChargingStation.ChargingProfiles.Models.Responses;

public class ChargingProfileResponse : BaseResponse, ITimeMarkable
{
    public int ChargingProfileId { get; set; }

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
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<ChargingSchedulePeriodResponse> ChargingSchedulePeriods { get; set; }
}