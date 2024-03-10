using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Connectors.Models.Requests;
using ChargingStation.Connectors.Models.Responses;

namespace ChargingStation.Connectors.Services;

public interface IConnectorService
{
    Task UpdateConnectorStatusAsync(UpdateConnectorStatusRequest request, CancellationToken cancellationToken = default);
    Task<ConnectorResponse> GetOrCreateConnectorAsync(GetOrCreateConnectorRequest request, CancellationToken cancellationToken = default);
    Task<ConnectorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StatusNotificationResponse> ProcessStatusNotificationAsync(StatusNotificationRequest request, Guid chargePointId, CancellationToken cancellationToken = default);
}