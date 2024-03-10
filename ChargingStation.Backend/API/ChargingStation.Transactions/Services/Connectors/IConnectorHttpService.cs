using ChargingStation.Connectors.Models.Requests;
using ChargingStation.Connectors.Models.Responses;

namespace ChargingStation.Transactions.Services.Connectors;

public interface IConnectorHttpService
{
    Task<ConnectorResponse> GetOrCreateConnectorAsync(GetOrCreateConnectorRequest request, CancellationToken cancellationToken = default);
    Task UpdateConnectorStatusAsync(UpdateConnectorStatusRequest updateStatusRequest, CancellationToken cancellationToken = default);
    Task<ConnectorResponse> GetByIdAsync(Guid connectorId, CancellationToken cancellationToken = default);
}