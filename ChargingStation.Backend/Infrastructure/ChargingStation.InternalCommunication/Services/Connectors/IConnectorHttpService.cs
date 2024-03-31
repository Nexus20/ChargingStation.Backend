using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.Connectors.Responses;

namespace ChargingStation.InternalCommunication.Services.Connectors;

public interface IConnectorHttpService
{
    Task<ConnectorResponse> GetOrCreateConnectorAsync(GetOrCreateConnectorRequest request, CancellationToken cancellationToken = default);
    Task UpdateConnectorStatusAsync(UpdateConnectorStatusRequest updateStatusRequest, CancellationToken cancellationToken = default);
    Task<ConnectorResponse> GetByIdAsync(Guid connectorId, CancellationToken cancellationToken = default);
    Task<ConnectorResponse> GetAsync(Guid chargePointId, int connectorId, CancellationToken cancellationToken = default);
    Task<ConnectorStatusResponse> GetConnectorStatusAsync(Guid chargePointId, int connectorId, CancellationToken cancellationToken = default);
    Task<ConnectorStatusResponse> GetConnectorStatusAsync(Guid connectorId, CancellationToken cancellationToken = default);
}