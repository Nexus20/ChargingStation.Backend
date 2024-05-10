using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Heartbeats.Models;
using ChargingStation.Heartbeats.Models.Request;

namespace ChargingStation.Heartbeats.Services.Heartbeats;

public interface IHeartbeatService
{
    Task AddHeartbeatAsync(HeartbeatEntity request, CancellationToken cancellationToken = default);
    Task<HeartbeatEntity> GetByIdAsync(GetHeartbeatRequest request, CancellationToken cancellationToken = default);
    Task<List<HeartbeatEntity>> GetAsync(CancellationToken cancellationToken = default);
    Task<HeartbeatResponse> ProcessHeartbeatAsync(HeartbeatRequest request, Guid chargePointId, CancellationToken cancellationToken = default);
}