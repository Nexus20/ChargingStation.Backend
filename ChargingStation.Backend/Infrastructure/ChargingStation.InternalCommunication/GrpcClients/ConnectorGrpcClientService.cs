using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.InternalCommunication.Extensions;
using Connectors.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;

namespace ChargingStation.InternalCommunication.GrpcClients;

public class ConnectorGrpcClientService
{
    private readonly ConnectorsGrpc.ConnectorsGrpcClient _connectorsGrpcClient;

    public ConnectorGrpcClientService(ConnectorsGrpc.ConnectorsGrpcClient connectorsGrpcClient)
    {
        _connectorsGrpcClient = connectorsGrpcClient;
    }
    
    public async Task<ConnectorResponse> GetByIdAsync(Guid connectorId, CancellationToken cancellationToken = default)
    {
        var request = new GetConnectorByIdGrpcRequest { Id = connectorId.ToString() };
        var grpcResponse = await _connectorsGrpcClient.GetByIdAsync(request, cancellationToken: cancellationToken);
        
        var response = grpcResponse.ToResponse();
        return response;
    }

    public async Task<List<ConnectorResponse>> GetByChargePointsIdsAsync(IEnumerable<Guid> chargePointsIds, CancellationToken cancellationToken = default)
    {
        var request = new GetConnectorsByChargePointsIdsGrpcRequest
        {
            ChargePointsIds = { chargePointsIds.Select(id => id.ToString()) }
        };
        
        var grpcResponse = await _connectorsGrpcClient.GetByChargePointsIdsAsync(request, cancellationToken: cancellationToken);
        
        var connectors = grpcResponse.Connectors.Select(connector => connector.ToResponse()).ToList();
        return connectors;
    }
    
    public async Task<ConnectorResponse> GetByChargePointIdAsync(Guid chargePointId, int connectorId,
        CancellationToken cancellationToken = default)
    {
        var request = new GetConnectorByChargePointIdGrpcRequest
        {
            ChargePointId = chargePointId.ToString(),
            ConnectorId = connectorId
        };
    
        var grpcResponse = await _connectorsGrpcClient.GetByChargePointIdAsync(request, cancellationToken: cancellationToken);
        
        var response = grpcResponse.ToResponse();
        return response;
    }

    public async Task UpdateConnectorStatusAsync(UpdateConnectorStatusRequest request, CancellationToken cancellationToken = default)
    {
        var grpcRequest = new UpdateConnectorStatusGrpcRequest
        {
            ChargePointId = request.ChargePointId.ToString(),
            ConnectorId = request.ConnectorId,
            Status = request.Status,
            ErrorCode = request.ErrorCode,
            Info = request.Info,
            StatusTimestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(request.StatusTimestamp, DateTimeKind.Utc)),
            VendorErrorCode = request.VendorErrorCode,
            VendorId = request.VendorId
        };
        
        await _connectorsGrpcClient.UpdateConnectorStatusAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task<ConnectorResponse> GetOrCreateConnectorAsync(GetOrCreateConnectorRequest connectorRequest, CancellationToken cancellationToken)
    {
        var grpcRequest = new GetOrCreateConnectorGrpcRequest
        {
            ChargePointId = connectorRequest.ChargePointId.ToString(),
            ConnectorId = connectorRequest.ConnectorId
        };
        
        var grpcResponse = await _connectorsGrpcClient.GetOrCreateConnectorAsync(grpcRequest, cancellationToken: cancellationToken);
        
        var response = grpcResponse.ToResponse();
        return response;
    }
}