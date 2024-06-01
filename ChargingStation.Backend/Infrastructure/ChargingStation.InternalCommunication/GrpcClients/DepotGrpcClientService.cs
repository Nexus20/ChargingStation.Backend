using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.InternalCommunication.Extensions;
using Depots.Grpc.Protos;

namespace ChargingStation.InternalCommunication.GrpcClients;

public class DepotGrpcClientService
{
    private readonly DepotsGrpc.DepotsGrpcClient _depotsGrpcClient;

    public DepotGrpcClientService(DepotsGrpc.DepotsGrpcClient depotsGrpcClient)
    {
        _depotsGrpcClient = depotsGrpcClient;
    }
    
    public async Task<DepotResponse> GetByIdAsync(Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var request = new GetDepotByIdGrpcRequest { Id = chargePointId.ToString() };
        var grpcResponse = await _depotsGrpcClient.GetByIdAsync(request, cancellationToken: cancellationToken);
        
        var response = grpcResponse.ToResponse();
        return response;
    }
}