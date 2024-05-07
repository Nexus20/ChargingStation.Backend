using ChargingStation.Common.Messages_OCPP16.Requests.Enums;

namespace Connectors.Application.Models.Requests;

public class ChangeConnectorAvailabilityRequest
{
    public required Guid ConnectorId { get; set; }
    public required ChangeAvailabilityRequestType AvailabilityType { get; set; }
}