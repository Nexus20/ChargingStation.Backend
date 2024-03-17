using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Heartbeats.Models;
using ChargingStation.Heartbeats.Models.Request;
using ChargingStation.Heartbeats.Services.Connectors;
using ChargingStation.Infrastructure.AzureTableStorage;

namespace ChargingStation.Heartbeats.Services.HeartbeatService;

public class HeartbeatService : IHeartbeatService
{
    private readonly IChargePointHttpService _chargePointService;
    private readonly ITableManager<HeartbeatEntity> _tableManager;
    private readonly string _tableName;

    public HeartbeatService(IChargePointHttpService chargePointService, ITableManager<HeartbeatEntity> tableManager, IConfiguration configuration)
    {
            _chargePointService = chargePointService;
            _tableManager = tableManager;
            _tableName = configuration.GetConnectionString("AzureTableStorage:HeartbeatTable")!;
        }

    public async Task AddHeartbeatAsync(HeartbeatEntity request, CancellationToken cancellationToken = default)
    {
            var chargePoint = await _chargePointService.GetByIdAsync(request.PartitionKey, cancellationToken);

            if (!chargePoint)
            {
                throw new NotFoundException(
                    $"ChargePointId {request.PartitionKey} not found");
            }

            await _tableManager.AddEntityAsync(_tableName, request);
        }

    public async Task<HeartbeatEntity> GetByIdAsync(GetHeartbeatRequest request, CancellationToken cancellationToken = default)
    {
            var heartbeat = await _tableManager.GetEntityAsync(_tableName, request.PartitionKey, request.RowKey);

            if (heartbeat is null)
                throw new NotFoundException(nameof(HeartbeatEntity), request);

            return heartbeat;
        }

    public async Task<List<HeartbeatEntity>> GetAsync(CancellationToken cancellationToken = default)
    {
            var heartbeats = await _tableManager.GetAllEntitiesAsync(_tableName);

            if (!heartbeats.Any())
                return Enumerable.Empty<HeartbeatEntity>().ToList();

            return heartbeats;
        }

    public async Task<HeartbeatResponse> ProcessHeartbeatAsync(HeartbeatRequest request, Guid chargePointId, CancellationToken cancellationToken = default)
    {
            DateTimeOffset currentTime = DateTimeOffset.UtcNow;

            var response = new HeartbeatResponse(currentTime);
            var createHeartbeatRequest = new HeartbeatEntity(chargePointId.ToString(), currentTime);

            await AddHeartbeatAsync(createHeartbeatRequest, cancellationToken);

            return response;
        }
}