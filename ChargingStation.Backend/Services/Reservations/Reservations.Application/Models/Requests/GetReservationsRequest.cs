using ChargingStation.Common.Models.General.Requests;

namespace Reservations.Application.Models.Requests;

public class GetReservationsRequest : BaseCollectionRequest
{
    public Guid? DepotId { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? ExpiryDateTime { get; set; }
    public Guid? ChargePointId { get; set; }
    public Guid? ConnectorId { get; set; }
    public Guid? TagId { get; set; }
    public string? Status { get; set; }
    public bool IsCancelled { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}