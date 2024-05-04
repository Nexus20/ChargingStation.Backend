using ChargingStation.Connectors.Services;
using Connectors.Grpc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ChargingStation.Connectors.GrpcServices;

public class ConnectorGrpcService : ConnectorsGrpc.ConnectorsGrpcBase
{
    private readonly IConnectorService _connectorService;

    public ConnectorGrpcService(IConnectorService connectorService)
    {
        _connectorService = connectorService;
    }

    public override async Task<ConnectorGrpcResponse> GetById(GetConnectorByIdGrpcRequest request, ServerCallContext context)
    {
        var result = await _connectorService.GetByIdAsync(Guid.Parse(request.Id), context.CancellationToken);
        
        var grpcResponse = new ConnectorGrpcResponse
        {
            Id = result.Id.ToString(),
            ChargePointId = result.ChargePointId.ToString(),
            ConnectorId = result.ConnectorId,
            CreatedAt = Timestamp.FromDateTime(result.CreatedAt),
            CurrentStatus = result.CurrentStatus != null ? new ConnectorStatusGrpcResponse
            {
                ConnectorId = result.CurrentStatus.ConnectorId.ToString(),
                CurrentStatus = result.CurrentStatus.CurrentStatus,
                ErrorCode = result.CurrentStatus.ErrorCode,
                Info = result.CurrentStatus.Info,
                StatusUpdatedTimestamp = result.CurrentStatus.StatusUpdatedTimestamp.HasValue ? Timestamp.FromDateTime(result.CurrentStatus.StatusUpdatedTimestamp.Value) : null,
                VendorErrorCode = result.CurrentStatus.VendorErrorCode,
                VendorId = result.CurrentStatus.VendorId
            } : null
        };
        
        return grpcResponse;
    }
}