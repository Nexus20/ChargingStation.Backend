using ChargingStation.Common.Models.OcppTags.Responses;
using ChargingStation.InternalCommunication.Extensions;
using ChargingStation.OcppTags.Grpc.Protos;

namespace ChargingStation.InternalCommunication.GrpcClients;

public class OcppTagGrpcClientService
{
    private readonly OcppTagsGrpc.OcppTagsGrpcClient _ocppTagsGrpcClient;

    public OcppTagGrpcClientService(OcppTagsGrpc.OcppTagsGrpcClient ocppTagsGrpcClient)
    {
        _ocppTagsGrpcClient = ocppTagsGrpcClient;
    }
    
    public async Task<OcppTagResponse> GetByIdAsync(Guid ocppTagId, CancellationToken cancellationToken = default)
    {
        var request = new GetOcppTagByIdGrpcRequest { Id = ocppTagId.ToString() };
        var grpcResponse = await _ocppTagsGrpcClient.GetByIdAsync(request, cancellationToken: cancellationToken);
        
        var response = grpcResponse.ToResponse();
        return response;
    }
    
    public async Task<OcppTagResponse?> GetByTagIdAsync(string tagId, CancellationToken cancellationToken = default)
    {
        var request = new GetOcppTagByOcppTagIdGrpcRequest { OcppTagId = tagId };
        var grpcResponse = await _ocppTagsGrpcClient.GetByTagIdAsync(request, cancellationToken: cancellationToken);

        if (grpcResponse is null)
            return null;
        
        var response = grpcResponse.ToResponse();
        return response;
    }
}