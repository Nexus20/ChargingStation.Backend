using ChargingStation.OcppTags.Grpc.Extensions;
using ChargingStation.OcppTags.Services;
using Grpc.Core;
using OcppTags.Grpc;

namespace ChargingStation.OcppTags.Grpc.Services;

public class OcppTagGrpcService : OcppTagsGrpc.OcppTagsGrpcBase
{
    private readonly IOcppTagService _ocppTagService;

    public OcppTagGrpcService(IOcppTagService ocppTagService)
    {
        _ocppTagService = ocppTagService;
    }

    public override async Task<OcppTagGrpcResponse> GetById(GetOcppTagByIdGrpcRequest request, ServerCallContext context)
    {
        var response = await _ocppTagService.GetByIdAsync(Guid.Parse(request.Id), context.CancellationToken);

        var grpcResponse = response.ToGrpcResponse();

        return grpcResponse;
    }

    public override async Task<OcppTagGrpcResponse?> GetByTagId(GetOcppTagByOcppTagIdGrpcRequest request, ServerCallContext context)
    {
        var response = await _ocppTagService.GetByOcppTagIdAsync(request.OcppTagId, context.CancellationToken);
        
        if(response is null)
            return null;
        
        var grpcResponse = response.ToGrpcResponse();

        return grpcResponse;
    }
}