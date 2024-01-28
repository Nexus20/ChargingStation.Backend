namespace ChargingStation.Domain.Abstract;

public interface ITimeMarkedEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}