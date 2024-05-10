using EnergyConsumption.Application.Services;
using EnergyConsumption.Grpc.Extensions;
using EnergyConsumption.Grpc.Protos;
using Grpc.Core;

namespace EnergyConsumption.Grpc.Services;

public class EnergyConsumptionSettingsGrpcService : EnergyConsumptionSettingsGrpc.EnergyConsumptionSettingsGrpcBase
{
    private readonly IEnergyConsumptionSettingsService _energyConsumptionSettingsService;

    public EnergyConsumptionSettingsGrpcService(IEnergyConsumptionSettingsService energyConsumptionSettingsService)
    {
        _energyConsumptionSettingsService = energyConsumptionSettingsService;
    }

    public override async Task<DepotEnergyConsumptionSettingsGrpcResponse> GetEnergyConsumptionSettingsByDepotId(GetByDepotIdGrpcRequest request, ServerCallContext context)
    {
        var depotId = Guid.Parse(request.DepotId);
        
        var response = await _energyConsumptionSettingsService.GetByDepotIdAsync(depotId);
        
        return response?.ToGrpcResponse() ?? new DepotEnergyConsumptionSettingsGrpcResponse();
    }

    public override async Task<DepotEnergyConsumptionSettingsGrpcResponse> GetEnergyConsumptionSettingsByChargePointId(GetByChargePointIdGrpcRequest request, ServerCallContext context)
    {
        var chargePointId = Guid.Parse(request.ChargePointId);
        
        var response = await _energyConsumptionSettingsService.GetByChargingStationIdAsync(chargePointId);
        
        return response?.ToGrpcResponse() ?? new DepotEnergyConsumptionSettingsGrpcResponse();
    }
}