using ChargingStation.Common.Models.Connectors.Responses;
using Connectors.Grpc;

namespace ChargingStation.InternalCommunication.GrpcClients;

public class ConnectorsGrpcClientService
{
    private readonly ConnectorsGrpc.ConnectorsGrpcClient _connectorsGrpcClient;

    public ConnectorsGrpcClientService(ConnectorsGrpc.ConnectorsGrpcClient connectorsGrpcClient)
    {
        _connectorsGrpcClient = connectorsGrpcClient;
    }
    
    public async Task<ConnectorResponse> GetByIdAsync(Guid connectorId, CancellationToken cancellationToken = default)
    {
        var request = new GetConnectorByIdGrpcRequest { Id = connectorId.ToString() };
        var grpcResponse = await _connectorsGrpcClient.GetByIdAsync(request, cancellationToken: cancellationToken);
        var response = new ConnectorResponse
        {
            Id = Guid.Parse(grpcResponse.Id),
            ChargePointId = Guid.Parse(grpcResponse.ChargePointId),
            ConnectorId = grpcResponse.ConnectorId,
            CreatedAt = grpcResponse.CreatedAt.ToDateTime(),
            CurrentStatus = grpcResponse.CurrentStatus != null ? new ConnectorStatusResponse
            {
                ConnectorId = Guid.Parse(grpcResponse.CurrentStatus.ConnectorId),
                CurrentStatus = grpcResponse.CurrentStatus.CurrentStatus,
                ErrorCode = grpcResponse.CurrentStatus.ErrorCode,
                Info = grpcResponse.CurrentStatus.Info,
                StatusUpdatedTimestamp = grpcResponse.CurrentStatus.StatusUpdatedTimestamp?.ToDateTime(),
                VendorErrorCode = grpcResponse.CurrentStatus.VendorErrorCode,
                VendorId = grpcResponse.CurrentStatus.VendorId
            } : null
        };
        
        return response;
    }
}