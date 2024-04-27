using ChargingStation.Common.Messages_OCPP16.Requests.Enums;

namespace ChargingStation.Connectors.Models.Requests;

public class ChangeConnectorAvailabilityRequest
{
    public required Guid ConnectorId { get; set; }
    public required ChangeAvailabilityRequestType AvailabilityType { get; set; }
}