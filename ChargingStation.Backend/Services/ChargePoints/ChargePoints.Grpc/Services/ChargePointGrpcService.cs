using ChargePoints.Application.Services;
using ChargePoints.Grpc.Extensions;
using ChargePoints.Grpc.Protos;
using ChargingStation.Common.Messages_OCPP16.Requests.Enums;
using ChargingStation.Common.Models.ChargePoints.Requests;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ChargePoints.Grpc.Services;

public class ChargePointGrpcService : ChargePointsGrpc.ChargePointsGrpcBase
{
    private readonly IChargePointService _chargePointService;

    public ChargePointGrpcService(IChargePointService chargePointService)
    {
        _chargePointService = chargePointService;
    }

    public override async Task<ChargePointGrpcResponse> GetById(GetChargePointByIdGrpcRequest request, ServerCallContext context)
    {
        var chargePointId = Guid.Parse(request.Id);
        var response = await _chargePointService.GetByIdAsync(chargePointId, context.CancellationToken);
        
        var grpcResponse = response.ToGrpcResponse();
        return grpcResponse;
    }

    public override async Task<ChargePointGrpcResponses> GetByIds(GetChargePointByIdsGrpcRequest request, ServerCallContext context)
    {
        var chargePointsIds = request.Ids.Select(Guid.Parse).ToList();
        var chargePoints = await _chargePointService.GetByIdsAsync(chargePointsIds, context.CancellationToken);
        
        var grpcResponse = new ChargePointGrpcResponses
        {
            ChargePoints = { chargePoints.Select(x => x.ToGrpcResponse()) }
        };
        
        return grpcResponse;
    }

    public override async Task<Empty> ChangeAvailability(ChangeChargePointAvailabilityGrpcRequest request, ServerCallContext context)
    {
        var changeAvailabilityRequest = new ChangeChargePointAvailabilityRequest
        {
            ChargePointId = Guid.Parse(request.ChargePointId),
            AvailabilityType = (ChangeAvailabilityRequestType)request.AvailabilityType
        };
        
        await _chargePointService.ChangeAvailabilityAsync(changeAvailabilityRequest, context.CancellationToken);
        return new Empty();
    }

    public override async Task<ChargePointGrpcResponses> GetByDepots(GetChargePointByDepotsGrpcRequest request, ServerCallContext context)
    {
        var depotsIds = request.DepotsIds.Select(Guid.Parse).ToList();
        
        var chargePoints = await _chargePointService.GetByDepotsIdsAsync(depotsIds, context.CancellationToken);
        
        var grpcResponse = new ChargePointGrpcResponses
        {
            ChargePoints = { chargePoints.Select(x => x.ToGrpcResponse()) }
        };
        
        return grpcResponse;
    }
}