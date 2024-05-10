using ChargingStation.Common.Models.DepotEnergyConsumption;
using ChargingStation.InternalCommunication.Extensions;
using EnergyConsumption.Grpc.Protos;

namespace ChargingStation.InternalCommunication.GrpcClients;

public class EnergyConsumptionSettingsGrpcClientService
{
    private readonly EnergyConsumptionSettingsGrpc.EnergyConsumptionSettingsGrpcClient _energyConsumptionSettingsGrpcClient;

    public EnergyConsumptionSettingsGrpcClientService(EnergyConsumptionSettingsGrpc.EnergyConsumptionSettingsGrpcClient energyConsumptionSettingsGrpcClient)
    {
        _energyConsumptionSettingsGrpcClient = energyConsumptionSettingsGrpcClient;
    }
    
    public async Task<DepotEnergyConsumptionSettingsResponse?> GetEnergyConsumptionSettingsByDepotAsync(Guid depotId, CancellationToken cancellationToken = default)
    {
        var grpcRequest = new GetByDepotIdGrpcRequest
        {
            DepotId = depotId.ToString()
        };
        
        var grpcResponse = await _energyConsumptionSettingsGrpcClient.GetEnergyConsumptionSettingsByDepotIdAsync(grpcRequest, cancellationToken: cancellationToken);
        
        if(CheckIfResponseIsEmpty(grpcResponse))
            return null;
        
        var response = grpcResponse.ToResponse();
        return response;
    }

    public async Task<DepotEnergyConsumptionSettingsResponse?> GetByChargingStationIdAsync(Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var grpcRequest = new GetByChargePointIdGrpcRequest
        {
            ChargePointId = chargePointId.ToString()
        };
        
        var grpcResponse = await _energyConsumptionSettingsGrpcClient.GetEnergyConsumptionSettingsByChargePointIdAsync(grpcRequest, cancellationToken: cancellationToken);
        
        if(CheckIfResponseIsEmpty(grpcResponse))
            return null;
        
        var response = grpcResponse.ToResponse();
        return response;
    }
    
    private bool CheckIfResponseIsEmpty(DepotEnergyConsumptionSettingsGrpcResponse grpcResponse)
    {
        return grpcResponse.Intervals == null || grpcResponse.Intervals.Count == 0 || grpcResponse.ChargePointsLimits == null || grpcResponse.ChargePointsLimits.Count == 0;
    }
}