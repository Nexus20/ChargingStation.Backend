using ChargingStation.ChargePoints.Grpc.Protos;
using ChargingStation.Common.Models.ChargePoints.Requests;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.InternalCommunication.Extensions;

namespace ChargingStation.InternalCommunication.GrpcClients;

public class ChargePointGrpcClientService
{
    private readonly ChargePointsGrpc.ChargePointsGrpcClient _chargePointsGrpcClient;

    public ChargePointGrpcClientService(ChargePointsGrpc.ChargePointsGrpcClient chargePointsGrpcClient)
    {
        _chargePointsGrpcClient = chargePointsGrpcClient;
    }
    
    public async Task<ChargePointResponse> GetByIdAsync(Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var request = new GetChargePointByIdGrpcRequest { Id = chargePointId.ToString() };
        var grpcResponse = await _chargePointsGrpcClient.GetByIdAsync(request, cancellationToken: cancellationToken);
        
        var response = grpcResponse.ToResponse();
        return response;
    }
    
    public async Task<List<ChargePointResponse>> GetByIdsAsync(IEnumerable<Guid> chargePointIds, CancellationToken cancellationToken = default)
    {
        var request = new GetChargePointByIdsGrpcRequest { Ids = { chargePointIds.Select(id => id.ToString()) } };
        var grpcResponse = await _chargePointsGrpcClient.GetByIdsAsync(request, cancellationToken: cancellationToken);
        
        var response = grpcResponse.ChargePoints.Select(c => c.ToResponse()).ToList();
        return response;
    }
    
    public async Task ChangeAvailabilityAsync(ChangeChargePointAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        var grpcRequest = new ChangeChargePointAvailabilityGrpcRequest()
        {
            ChargePointId = request.ChargePointId.ToString(),
            AvailabilityType = (int)request.AvailabilityType
        };
        
        await _chargePointsGrpcClient.ChangeAvailabilityAsync(grpcRequest, cancellationToken: cancellationToken);
    }
}