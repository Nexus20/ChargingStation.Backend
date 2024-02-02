namespace ChargingStation.Common.Models.Requests;

public abstract class BaseCollectionRequest
{
    public PagePredicate? PagePredicate { get; set; }
    public List<OrderPredicate> OrderPredicates { get; set; } = [];
}