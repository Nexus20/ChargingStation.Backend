using Depots.Application.Services;
using Depots.Grpc.Extensions;
using Depots.Grpc.Protos;
using Grpc.Core;

namespace Depots.Grpc.Services;

public class DepotGrpcService : DepotsGrpc.DepotsGrpcBase
{
    private readonly IDepotService _depotService;

    public DepotGrpcService(IDepotService depotService)
    {
        _depotService = depotService;
    }

    public override async Task<DepotGrpcResponse> GetById(GetDepotByIdGrpcRequest request, ServerCallContext context)
    {
        var response = await _depotService.GetByIdAsync(Guid.Parse(request.Id), context.CancellationToken);
        
        var grpcResponse = response.ToGrpcResponse();
        return grpcResponse;
    }
}