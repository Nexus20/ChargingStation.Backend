using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Heartbeats.Models;
using ChargingStation.Heartbeats.Models.Request;
using ChargingStation.InternalCommunication.GrpcClients;
using ChargingStation.TableStorage.Managers;

namespace ChargingStation.Heartbeats.Services.Heartbeats;

public class HeartbeatService : IHeartbeatService
{
    private readonly ChargePointGrpcClientService _chargePointGrpcClientService;
    private readonly ITableManager<HeartbeatEntity> _tableManager;
    private readonly string _tableName;

    public HeartbeatService(ChargePointGrpcClientService chargePointGrpcClientService, ITableManager<HeartbeatEntity> tableManager,
        IConfiguration configuration)
    {
        _chargePointGrpcClientService = chargePointGrpcClientService;
        _tableManager = tableManager;
        _tableName = configuration.GetValue<string>("TablesConfiguration:HeartbeatTable")!;
    }

    public async Task AddHeartbeatAsync(HeartbeatEntity request, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _chargePointGrpcClientService.GetByIdAsync(Guid.Parse(request.PartitionKey), cancellationToken);

        if (chargePoint is null)
        {
            throw new NotFoundException($"ChargePointId {request.PartitionKey} not found");
        }

        await _tableManager.AddEntityAsync(_tableName, request);
    }

    public async Task<HeartbeatEntity> GetByIdAsync(GetHeartbeatRequest request,
        CancellationToken cancellationToken = default)
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

    public async Task<HeartbeatResponse> ProcessHeartbeatAsync(HeartbeatRequest request, Guid chargePointId,
        CancellationToken cancellationToken = default)
    {
        var currentTime = DateTimeOffset.UtcNow;

        var response = new HeartbeatResponse(currentTime);
        var createHeartbeatRequest = new HeartbeatEntity(chargePointId.ToString(), currentTime);

        await AddHeartbeatAsync(createHeartbeatRequest, cancellationToken);

        return response;
    }
}