using ChargingStation.InternalCommunication.GrpcClients;
using Transactions.Application.Models.EnergyConsumption.Requests;
using Transactions.Application.Models.EnergyConsumption.Responses;
using Transactions.Application.Repositories.ConnectorMeterValues;

namespace Transactions.Application.Services.EnergyConsumption;

public class EnergyConsumptionService : IEnergyConsumptionService
{
    private readonly ChargePointGrpcClientService _chargePointGrpcClientService;
    private readonly ConnectorGrpcClientService _connectorGrpcClientService;
    private readonly IConnectorMeterValueRepository _connectorMeterValueRepository;

    public EnergyConsumptionService(ChargePointGrpcClientService chargePointGrpcClientService, ConnectorGrpcClientService connectorGrpcClientService, IConnectorMeterValueRepository connectorMeterValueRepository)
    {
        _chargePointGrpcClientService = chargePointGrpcClientService;
        _connectorGrpcClientService = connectorGrpcClientService;
        _connectorMeterValueRepository = connectorMeterValueRepository;
    }

    public async Task<ChargePointsEnergyConsumptionResponse> GetChargePointsEnergyConsumptionByDepotAsync(GetChargePointsEnergyConsumptionByDepotRequest request,
        CancellationToken cancellationToken = default)
    {
        var chargePoints = await _chargePointGrpcClientService.GetByDepotsIdsAsync(new List<Guid> { request.DepotId }, cancellationToken);
        var chargePointsIds = chargePoints.Select(x => x.Id).ToList();
        var connectors = await _connectorGrpcClientService.GetByChargePointsIdsAsync(chargePointsIds, cancellationToken);
        var connectorsIds = connectors.Select(x => x.Id).ToList();
        
        var connectorsConsumption = await _connectorMeterValueRepository.GetChargePointsConnectorsEnergyConsumptionByDepotAsync(connectorsIds, request.StartTime, request.EndTime);
        
        var response = new ChargePointsEnergyConsumptionResponse
        {
            ChargePointsConsumption = [],
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };
        
        foreach (var chargePoint in chargePoints)
        {
            var chargePointConsumption = new ChargePointEnergyConsumptionResponse
            {
                ChargePointId = chargePoint.Id,
                ChargePointName = chargePoint.Name,
                EnergyConsumed = 0,
                ConnectorsConsumption = []
            };
            
            var chargePointConnectors = connectors.Where(x => x.ChargePointId == chargePoint.Id).ToList();
            var chargePointConnectorsIds = chargePointConnectors.Select(x => x.Id).ToList();

            var chargePointConnectorsConsumption = connectorsConsumption.Where(x => chargePointConnectorsIds.Contains(x.ConnectorId)).ToList();
            chargePointConsumption.ConnectorsConsumption = chargePointConnectorsConsumption;
            chargePointConsumption.EnergyConsumed = chargePointConnectorsConsumption.Sum(x => x.EnergyConsumed);
            
            response.ChargePointsConsumption.Add(chargePointConsumption);
        }
        
        return response;
    }

    public async Task<List<DepotEnergyConsumptionStatisticsResponse>> GetDepotEnergyConsumption(GetDepotEnergyConsumptionStatisticsRequest request, CancellationToken cancellationToken = default)
    {
        var chargePoints = await _chargePointGrpcClientService.GetByDepotsIdsAsync(new List<Guid> { request.DepotId }, cancellationToken);
        var chargePointsIds = chargePoints.Select(x => x.Id).ToList();
        var connectors = await _connectorGrpcClientService.GetByChargePointsIdsAsync(chargePointsIds, cancellationToken);
        var connectorsIds = connectors.Select(x => x.Id).ToList();
        
        var depotEnergyConsumption = await _connectorMeterValueRepository.GetDepotEnergyConsumptionAsync(connectorsIds, request.AggregationInterval, request.StartTime, request.EndTime, cancellationToken);
        
        return depotEnergyConsumption;
    }
}