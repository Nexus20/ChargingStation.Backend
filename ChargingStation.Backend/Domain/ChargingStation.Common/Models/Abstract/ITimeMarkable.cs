namespace ChargingStation.Common.Models.Abstract;

public interface ITimeMarkable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}