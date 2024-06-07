using System.Data;
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

    public async Task<List<ChargePointConnectorEnergyConsumptionResponse>> GetChargePointsConnectorsEnergyConsumptionByDepotAsync(List<Guid> connectorsIds, DateTime? startTime, DateTime? endTime, CancellationToken cancellationToken = default)
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
            .ToListAsync(cancellationToken: cancellationToken);
        
        return connectorsEnergyConsumption;
    }
    
    public async Task<List<DepotEnergyConsumptionStatisticsResponse>> GetDepotEnergyConsumptionAsync(List<Guid> connectorsIds, TimeSpan aggregationInterval, DateTime? startTime, DateTime? endTime, CancellationToken cancellationToken = default)
    {
        var connectorsIdsString = string.Join(",", connectorsIds.Select(id => $"'{id}'"));
        var intervalMinutes = (int)aggregationInterval.TotalMinutes;

        var sqlQuery = $@"
            WITH AggregatedData AS (
                SELECT 
                    DATEADD(MINUTE, (DATEDIFF(MINUTE, 0, cmv.MeterValueTimestamp) / {intervalMinutes}) * {intervalMinutes}, 0) AS TimeInterval,
                    SUM(CAST(cmv.Value AS DECIMAL(10, 2))) AS AggregatedValue
                FROM 
                    ConnectorMeterValue cmv
                    JOIN Connectors c ON cmv.ConnectorId = c.Id
                    JOIN ChargePoints cp ON c.ChargePointId = cp.Id
                    JOIN Depots d ON cp.DepotId = d.Id
                WHERE 
                    d.Id = 'ABB20DF1-A7BE-41B4-1B09-08DC76A4496B'
                    AND cmv.Measurand = 'Energy_Active_Import_Register'
                    AND cmv.ConnectorId IN ({connectorsIdsString})" +
                    (startTime.HasValue ? " AND cmv.MeterValueTimestamp >= @StartTime" : "") +
                    (endTime.HasValue ? " AND cmv.MeterValueTimestamp <= @EndTime" : "") + $@"
                GROUP BY 
                    DATEADD(MINUTE, (DATEDIFF(MINUTE, 0, cmv.MeterValueTimestamp) / {intervalMinutes}) * {intervalMinutes}, 0)
            )
            SELECT 
                TimeInterval,
                AggregatedValue,
                SUM(AggregatedValue) OVER (ORDER BY TimeInterval) AS CumulativeSum
            FROM 
                AggregatedData
            ORDER BY 
                TimeInterval;";

        await using var connection = DbContext.Database.GetDbConnection();
        
        await connection.OpenAsync(cancellationToken);

        using (var command = connection.CreateCommand())
        {
            command.CommandText = sqlQuery;
            command.CommandType = CommandType.Text;

            if (startTime.HasValue)
            {
                var startTimeParam = command.CreateParameter();
                startTimeParam.ParameterName = "@StartTime";
                startTimeParam.Value = startTime.Value;
                startTimeParam.DbType = DbType.DateTime;
                command.Parameters.Add(startTimeParam);
            }

            if (endTime.HasValue)
            {
                var endTimeParam = command.CreateParameter();
                endTimeParam.ParameterName = "@EndTime";
                endTimeParam.Value = endTime.Value;
                endTimeParam.DbType = DbType.DateTime;
                command.Parameters.Add(endTimeParam);
            }

            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                var statisticsResponse = new List<DepotEnergyConsumptionStatisticsResponse>();
                while (await reader.ReadAsync(cancellationToken))
                {
                    var result = new DepotEnergyConsumptionStatisticsResponse
                    {
                        DateTime = reader.GetDateTime(reader.GetOrdinal("TimeInterval")),
                        AggregatedValue = reader.GetDecimal(reader.GetOrdinal("AggregatedValue")),
                        CumulativeValue = reader.GetDecimal(reader.GetOrdinal("CumulativeSum"))
                    };
                    statisticsResponse.Add(result);
                }
                return statisticsResponse;
            }
        }
    }
}

