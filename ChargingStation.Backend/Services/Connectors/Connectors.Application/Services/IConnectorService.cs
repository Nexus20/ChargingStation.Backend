using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.Connectors.Responses;
using Connectors.Application.Models.Requests;

namespace Connectors.Application.Services;

public interface IConnectorService
{
    Task UpdateConnectorStatusAsync(UpdateConnectorStatusRequest request, CancellationToken cancellationToken = default);
    Task<ConnectorResponse> GetOrCreateConnectorAsync(GetOrCreateConnectorRequest request, CancellationToken cancellationToken = default);
    Task<ConnectorResponse> GetByChargePointIdAsync(GetConnectorByChargePointIdRequest request, CancellationToken cancellationToken = default);
    Task<ConnectorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StatusNotificationResponse> ProcessStatusNotificationAsync(StatusNotificationRequest request, Guid chargePointId, CancellationToken cancellationToken = default);
    Task<List<ConnectorResponse>> GetByChargePointsIdsAsync(List<Guid> chargePointsIds, CancellationToken cancellationToken = default);
    Task ChangeAvailabilityAsync(ChangeConnectorAvailabilityRequest request, CancellationToken cancellationToken = default);
}