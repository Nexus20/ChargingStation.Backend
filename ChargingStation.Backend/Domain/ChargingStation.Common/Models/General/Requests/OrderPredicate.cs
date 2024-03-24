namespace ChargingStation.Common.Models.General.Requests;

public class OrderPredicate
{
    public required string PropertyName { get; set; }
    public OrderDirection OrderDirection { get; set; }
}