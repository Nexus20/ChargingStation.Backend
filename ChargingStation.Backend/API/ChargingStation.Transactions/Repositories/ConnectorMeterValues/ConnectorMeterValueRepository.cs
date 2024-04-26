using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Persistence;
using ChargingStation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChargingStation.Transactions.Repositories.ConnectorMeterValues;

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
}