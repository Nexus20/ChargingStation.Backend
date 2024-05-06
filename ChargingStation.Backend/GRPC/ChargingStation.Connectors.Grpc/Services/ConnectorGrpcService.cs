using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Connectors.Grpc.Extensions;
using ChargingStation.Connectors.Services;
using Connectors.Grpc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ChargingStation.Connectors.Grpc.Services;

public class ConnectorGrpcService : ConnectorsGrpc.ConnectorsGrpcBase
{
    private readonly IConnectorService _connectorService;

    public ConnectorGrpcService(IConnectorService connectorService)
    {
        _connectorService = connectorService;
    }
    
    public override async Task<ConnectorGrpcResponse> GetById(GetConnectorByIdGrpcRequest request, ServerCallContext context)
    {
        var response = await _connectorService.GetByIdAsync(Guid.Parse(request.Id), context.CancellationToken);

        var grpcResponse = response.ToGrpcResponse();
        return grpcResponse;
    }

    public override async Task<ConnectorGrpcResponse> GetByChargePointId(GetConnectorByChargePointIdGrpcRequest request, ServerCallContext context)
    {
        var getConnectorByChargePointIdRequest = new GetConnectorByChargePointIdRequest
        {
            ChargePointId = Guid.Parse(request.ChargePointId),
            ConnectorId = request.ConnectorId
        };
        
        var response = await _connectorService.GetByChargePointIdAsync(getConnectorByChargePointIdRequest, context.CancellationToken);
        
        var grpcResponse = response.ToGrpcResponse();
        return grpcResponse;
    }

    public override async Task<Empty> UpdateConnectorStatus(UpdateConnectorStatusGrpcRequest request, ServerCallContext context)
    {
        var updateConnectorStatusRequest = new UpdateConnectorStatusRequest
        {
            ChargePointId = Guid.Parse(request.ChargePointId),
            Status = request.Status,
            ErrorCode = request.ErrorCode,
            Info = request.Info,
            VendorErrorCode = request.VendorErrorCode,
            VendorId = request.VendorId,
            ConnectorId = request.ConnectorId,
            StatusTimestamp = request.StatusTimestamp.ToDateTime(),
        };
        
        await _connectorService.UpdateConnectorStatusAsync(updateConnectorStatusRequest, context.CancellationToken);
        return new Empty();
    }

    public override async Task<ConnectorGrpcResponse> GetOrCreateConnector(GetOrCreateConnectorGrpcRequest request, ServerCallContext context)
    {
        var getOrCreateConnectorRequest = new GetOrCreateConnectorRequest
        {
            ChargePointId = Guid.Parse(request.ChargePointId),
            ConnectorId = request.ConnectorId
        };
        
        var response = await _connectorService.GetOrCreateConnectorAsync(getOrCreateConnectorRequest, context.CancellationToken);
        
        var grpcResponse = response.ToGrpcResponse();
        return grpcResponse;
    }
}