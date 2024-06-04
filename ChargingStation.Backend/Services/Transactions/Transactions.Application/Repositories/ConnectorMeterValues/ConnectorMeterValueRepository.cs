using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Persistence;
using ChargingStation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Transactions.Application.Models.Dtos;
using Transactions.Application.Models.EnergyConsumption.Responses;

namespace Transactions.Application.Repositories.ConnectorMeterValues;

public class ConnectorMeterValueRepository : Repository<ConnectorMeterValue>, IConnectorMeterValueRepository
{
    public ConnectorMeterValueRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<double> GetTotalEnergyConsumedByDepotAsync(Guid depotId, DateTime validFrom,
        CancellationToken cancellationToken = default)
    { 
        var meterValues = await DbSet.Include(x => x.Connector)
            .ThenInclude(x => x.ChargePoint)
            .Where(x => x.Connector.ChargePoint.DepotId == depotId
                        && x.MeterValueTimestamp >= validFrom
                        && x.Measurand == SampledValueMeasurand.Energy_Active_Import_Register.ToString())
            .ToListAsync(cancellationToken: cancellationToken);

        var consumedEnergy = meterValues.Sum(x => double.Parse(x.Value));
        return consumedEnergy;
    }

    public async Task<double> GetTotalEnergyConsumedByChargePointAsync(Guid chargePointId, DateTime validFrom,
        CancellationToken cancellationToken = default)
    {
        var meterValues = await DbSet.Include(x => x.Connector)
            .Where(x => x.Connector.ChargePointId == chargePointId
                        && x.MeterValueTimestamp >= validFrom
                        && x.Measurand == SampledValueMeasurand.Energy_Active_Import_Register.ToString())
            .ToListAsync(cancellationToken: cancellationToken);
        
        var consumedEnergy = meterValues.Sum(x => double.Parse(x.Value));
        return consumedEnergy;
    }
    
    public async Task<List<SoCDateTime>> GetSoCForTransactionAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var socValues = await DbSet
            .Where(x => x.TransactionId == transactionId
                        && x.Measurand == SampledValueMeasurand.SoC.ToString())
            .OrderBy(x => x.MeterValueTimestamp)
            .Select(x =>
                new SoCDateTime
                {
                    MeterValueTimestamp = x.MeterValueTimestamp,
                    SoCValue = Convert.ToDouble(x.Value)
                })
            .ToListAsync(cancellationToken: cancellationToken);
        
        return socValues;
    }

    public async Task<List<ChargePointConnectorEnergyConsumptionResponse>> GetChargePointsConnectorsEnergyConsumptionByDepotAsync(List<Guid> connectorsIds, DateTime? startTime, DateTime? endTime)
    {
        var query = DbSet
            .Include(x => x.Connector)
            .Where(x => x.Measurand == SampledValueMeasurand.Energy_Active_Import_Register.ToString() && connectorsIds.Contains(x.ConnectorId));
        
        if(startTime.HasValue)
            query = query.Where(x => x.MeterValueTimestamp >= startTime);
        
        if(endTime.HasValue)
            query = query.Where(x => x.MeterValueTimestamp <= endTime);

        var connectorsEnergyConsumption = await query.GroupBy(x => new { x.Connector!.Id, x.Connector.ConnectorId })
            .Select(x => new ChargePointConnectorEnergyConsumptionResponse()
            {
                ConnectorId = x.Key.Id,
                ConnectorNumber = x.Key.ConnectorId,
                EnergyConsumed = x.Sum(y => Convert.ToDouble(y.Value))
            })
            .ToListAsync();
        
        return connectorsEnergyConsumption;
    }
}