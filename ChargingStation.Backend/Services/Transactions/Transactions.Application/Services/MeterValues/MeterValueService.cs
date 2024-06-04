using System.Globalization;
using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using ChargingStation.InternalCommunication.GrpcClients;
using ChargingStation.InternalCommunication.SignalRModels;
using ChargingStation.Mailing.Messages;
using ChargingStation.Mailing.Services;
using MassTransit;
using MathNet.Numerics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Transactions.Application.Models.Dtos;
using Transactions.Application.Models.Requests;
using Transactions.Application.Repositories.ConnectorMeterValues;
using Transactions.Application.Specifications;

namespace Transactions.Application.Services.MeterValues;

public class MeterValueService : IMeterValueService
{
    private readonly ConnectorGrpcClientService _connectorGrpcClientService;
    private readonly EnergyConsumptionSettingsGrpcClientService _energyConsumptionSettingsGrpcClientService;
    private readonly IRepository<OcppTransaction> _transactionRepository;
    private readonly IConnectorMeterValueRepository _connectorMeterValueRepository;
    private readonly ILogger<MeterValueService> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IEmailService _emailService;

    public MeterValueService(ILogger<MeterValueService> logger, ConnectorGrpcClientService connectorGrpcClientService, IRepository<OcppTransaction> transactionRepository, IConnectorMeterValueRepository connectorMeterValueRepository, IPublishEndpoint publishEndpoint, EnergyConsumptionSettingsGrpcClientService energyConsumptionSettingsGrpcClientService, IEmailService emailService)
    {
        _logger = logger;
        _connectorGrpcClientService = connectorGrpcClientService;
        _transactionRepository = transactionRepository;
        _connectorMeterValueRepository = connectorMeterValueRepository;
        _publishEndpoint = publishEndpoint;
        _energyConsumptionSettingsGrpcClientService = energyConsumptionSettingsGrpcClientService;
        _emailService = emailService;
    }

    public async Task<MeterValuesResponse> ProcessMeterValueAsync(MeterValuesRequest request, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var response = new MeterValuesResponse();
        var connectorChangesMessage = new ConnectorChangesMessage();

        var connectorId = -1;

        try
        {
            connectorId = request.ConnectorId;
            
            var connector = await _connectorGrpcClientService.GetByChargePointIdAsync(chargePointId, connectorId, cancellationToken);
            
            var getTransactionRequest = new GetTransactionsRequest
            {
                TransactionId = request.TransactionId
            };
                
            var specification = new GetTransactionsSpecification(getTransactionRequest);
            var transaction = await _transactionRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
            
            if (transaction == null)
            {
                _logger.LogError("MeterValues => Transaction not found: {TransactionId}", request.TransactionId);
                return response;
            }

            var meterValuesToAdd = new List<ConnectorMeterValue>();
            
            // Known charge station => process meter values
            double? valueToSave = null;
            var calculateApproximateChargingEndTime = true;
            foreach (var meterValue in request.MeterValue)
            {
                foreach (var sampleValue in meterValue.SampledValue)
                {
                    _logger.LogTrace("MeterValues => Context={Context} / Format={Format} / Value={Value} / Unit={Unit} / Location={Location} / Measurand={Measurand} / Phase={Phase}",
                        sampleValue.Context, sampleValue.Format, sampleValue.Value, sampleValue.Unit, sampleValue.Location, sampleValue.Measurand, sampleValue.Phase);

                    if (sampleValue.Measurand == SampledValueMeasurand.Power_Active_Import)
                    {
                        // current charging power
                        if (double.TryParse(sampleValue.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var currentChargeW))
                        {
                            if (sampleValue.Unit is SampledValueUnit.W or SampledValueUnit.VA or SampledValueUnit.Var or null)
                            {
                                connectorChangesMessage.Power = currentChargeW;
                                valueToSave = currentChargeW;
                                _logger.LogTrace("MeterValues => Charging '{CurrentChargeW:0.0}' W", currentChargeW);
                            }
                            else if (sampleValue.Unit is SampledValueUnit.KW or SampledValueUnit.KVA or SampledValueUnit.Kvar)
                            {
                                connectorChangesMessage.Power = currentChargeW * 1000;
                                valueToSave = currentChargeW * 1000;
                                _logger.LogTrace("MeterValues => Charging '{CurrentChargeKw:0.0}' kW", currentChargeW);
                            }
                            else
                            {
                                _logger.LogWarning("MeterValues => Charging: unexpected unit: '{Unit}' (Value={Value})", sampleValue.Unit, sampleValue.Value);
                            }
                        }
                        else
                        {
                            _logger.LogError("MeterValues => Charging: invalid value '{Value}' (Unit={Unit})", sampleValue.Value, sampleValue.Unit);
                        }
                    }
                    else if (sampleValue.Measurand is SampledValueMeasurand.Energy_Active_Import_Register or null)
                    {
                        // charged amount of energy
                        if (double.TryParse(sampleValue.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var meterWh))
                        {
                            if (sampleValue.Unit is SampledValueUnit.Wh or SampledValueUnit.Varh or null)
                            {
                                connectorChangesMessage.Energy = meterWh;
                                valueToSave = meterWh;
                                _logger.LogTrace("MeterValues => Value: '{MeterWh:0.0}' Wh", meterWh);
                            }
                            else if (sampleValue.Unit is SampledValueUnit.KWh or SampledValueUnit.Kvarh)
                            {
                                connectorChangesMessage.Energy = meterWh * 1000;
                                valueToSave = meterWh * 1000;
                                _logger.LogTrace("MeterValues => Value: '{MeterKwh:0.0}' kWh", meterWh);
                            }
                            else
                            {
                                _logger.LogWarning("MeterValues => Value: unexpected unit: '{Unit}' (Value={Value})", sampleValue.Unit, sampleValue.Value);
                            }
                        }
                        else
                        {
                            _logger.LogError("MeterValues => Value: invalid value '{Value}' (Unit={Unit})", sampleValue.Value, sampleValue.Unit);
                        }
                    }
                    else if (sampleValue.Measurand == SampledValueMeasurand.SoC)
                    {
                        // state of charge (battery status)
                        if (double.TryParse(sampleValue.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var stateOfCharge))
                        {
                            if(stateOfCharge == 100)
                                calculateApproximateChargingEndTime = false;
                            
                            connectorChangesMessage.SoC = stateOfCharge;
                            valueToSave = stateOfCharge;
                            _logger.LogTrace("MeterValues => SoC: '{0:0.0}'%", stateOfCharge);
                        }
                        else
                        {
                            _logger.LogError("MeterValues => invalid value '{Value}' (SoC)", sampleValue.Value);
                        }
                    }

                    if (valueToSave.HasValue)
                    {
                        var meterValueToAdd = new ConnectorMeterValue
                        {
                            ConnectorId = connector.Id,
                            TransactionId = transaction.Id,
                            Value = valueToSave.Value.ToString(CultureInfo.InvariantCulture),
                            Measurand = sampleValue.Measurand!.Value.ToString(),
                            Location = sampleValue.Location.ToString(),
                            Phase = sampleValue.Phase.ToString(),
                            Format = sampleValue.Format.ToString(),
                            Unit = sampleValue.Unit.ToString(),
                            MeterValueTimestamp = meterValue.Timestamp.UtcDateTime,
                        };
                    
                        meterValuesToAdd.Add(meterValueToAdd);
                    }
                }
                
                if (meterValuesToAdd.Count > 0)
                {
                    await _connectorMeterValueRepository.AddRangeAsync(meterValuesToAdd, cancellationToken);
                    await _connectorMeterValueRepository.SaveChangesAsync(cancellationToken);
                    
                    connectorChangesMessage.ChargePointId = chargePointId;
                    connectorChangesMessage.ConnectorId = connector.Id;
                    connectorChangesMessage.TransactionId = transaction.TransactionId;

                    if (calculateApproximateChargingEndTime)
                    {
                        var endTime = await CalculateChargingEndTime(transaction.Id, transaction.StartTime, cancellationToken);
                        connectorChangesMessage.ApproximateChargingEndTime = endTime;
                    }

                    var signalRMessage = new SignalRMessage(JsonConvert.SerializeObject(connectorChangesMessage), nameof(ConnectorChangesMessage));
                    await _publishEndpoint.Publish(signalRMessage, cancellationToken);
                 
                    await CheckEnergyConsumptionLimitAndWarnAsync(chargePointId, cancellationToken);
                    
                    return response;
                }

                _logger.LogWarning("MeterValues => No values to save");
            }
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "MeterValues => Exception: {Message}", exp.Message);
        }

        return response;
    }
    
    private async Task<DateTime> CalculateChargingEndTime(Guid transactionId, DateTime transactionStartTime, CancellationToken cancellationToken = default)
    {
        var meterValues = new List<SoCDateTime>
        {
            new()
            {
                MeterValueTimestamp = transactionStartTime,
                SoCValue = 0
            }
        };
        
        var meterValuesFromDb = await _connectorMeterValueRepository.GetSoCForTransactionAsync(transactionId, cancellationToken);
        meterValues.AddRange(meterValuesFromDb);
        
        var times = meterValues.Select(x => (x.MeterValueTimestamp - transactionStartTime).TotalSeconds).ToArray();
        var charges = meterValues.Select(x => x.SoCValue).ToArray();
        
        var (intercept, slope) = Fit.Line(times, charges);
        
        var remainingCharge = 100 - charges[^1];
        var timeToFullCharge = (remainingCharge - intercept) / slope;
        
        var endTime = transactionStartTime.AddSeconds(times[^1] + timeToFullCharge);
        return endTime;
    }
    
    private async Task CheckEnergyConsumptionLimitAndWarnAsync(Guid chargePointId,
        CancellationToken cancellationToken = default)
    {
        var energyConsumptionSettings =
            await _energyConsumptionSettingsGrpcClientService.GetByChargingStationIdAsync(chargePointId, cancellationToken);

        if (energyConsumptionSettings is null)
            return;

        var depotLimit = energyConsumptionSettings.DepotEnergyLimit;
        var chargePointLimit = energyConsumptionSettings.ChargePointsLimits.First(x => x.ChargePointId == chargePointId).ChargePointEnergyLimit;
        var currentInterval = energyConsumptionSettings.Intervals.First(x => x.StartTime <= DateTime.UtcNow && x.EndTime >= DateTime.UtcNow);
        var currentIntervalLimit = currentInterval.EnergyLimit;

        var totalEnergyConsumedByDepot = await _connectorMeterValueRepository.GetTotalEnergyConsumedByDepotAsync(energyConsumptionSettings.DepotId, currentInterval.StartTime, cancellationToken);
        var totalEnergyConsumedByDepotInCurrentInterval = await _connectorMeterValueRepository.GetTotalEnergyConsumedByDepotAsync(energyConsumptionSettings.DepotId, currentInterval.StartTime, cancellationToken);

        var totalEnergyConsumedByChargePoint = await _connectorMeterValueRepository.GetTotalEnergyConsumedByChargePointAsync(chargePointId, currentInterval.StartTime, cancellationToken);
        var totalEnergyConsumedByChargePointInCurrentInterval = await _connectorMeterValueRepository.GetTotalEnergyConsumedByChargePointAsync(chargePointId, currentInterval.StartTime, cancellationToken);

        if (totalEnergyConsumedByDepot > depotLimit)
        {
            _logger.LogWarning("MeterValues => Depot energy limit exceeded: {TotalEnergyConsumedByDepot} > {DepotLimit}", totalEnergyConsumedByDepot, depotLimit);
            var warningEmailMessage = new EnergyConsumptionWarningEmailMessage(energyConsumptionSettings.DepotId, chargePointId, DateTime.UtcNow, totalEnergyConsumedByDepot, depotLimit);
            await _emailService.SendMessageAsync(warningEmailMessage, cancellationToken: cancellationToken);

            var energyLimitExceededMessage = new EnergyLimitExceededMessage
            {
                ChargePointId = chargePointId,
                DepotId = energyConsumptionSettings.DepotId,
                EnergyConsumptionLimit = depotLimit,
                EnergyConsumption = totalEnergyConsumedByDepot,
                WarningTimestamp = DateTime.UtcNow
            };

            var energyLimitExceededSignalRMessage = new SignalRMessage(JsonConvert.SerializeObject(energyLimitExceededMessage), nameof(EnergyLimitExceededMessage));
            await _publishEndpoint.Publish(energyLimitExceededSignalRMessage, cancellationToken);
        } 
        else if (totalEnergyConsumedByDepotInCurrentInterval > currentIntervalLimit)
        {
            _logger.LogWarning("MeterValues => Depot energy limit exceeded for current interval: {TotalEnergyConsumedByDepot} > {CurrentIntervalLimit}", totalEnergyConsumedByDepot, currentIntervalLimit);
            var warningEmailMessage = new EnergyConsumptionWarningEmailMessage(energyConsumptionSettings.DepotId, chargePointId, DateTime.UtcNow, totalEnergyConsumedByDepot, currentIntervalLimit);
            await _emailService.SendMessageAsync(warningEmailMessage, cancellationToken: cancellationToken);

            var energyLimitExceededMessage = new EnergyLimitExceededMessage
            {
                ChargePointId = chargePointId,
                DepotId = energyConsumptionSettings.DepotId,
                EnergyConsumptionLimit = currentIntervalLimit,
                EnergyConsumption = totalEnergyConsumedByDepotInCurrentInterval,
                WarningTimestamp = DateTime.UtcNow
            };

            var energyLimitExceededSignalRMessage = new SignalRMessage(JsonConvert.SerializeObject(energyLimitExceededMessage), nameof(EnergyLimitExceededMessage));
            await _publishEndpoint.Publish(energyLimitExceededSignalRMessage, cancellationToken);
        }
        else if (totalEnergyConsumedByChargePoint > chargePointLimit)
        {
            _logger.LogWarning("MeterValues => Charge point energy limit exceeded: {TotalEnergyConsumedByChargePoint} > {ChargePointLimit}", totalEnergyConsumedByChargePoint, chargePointLimit);
            var warningEmailMessage = new EnergyConsumptionWarningEmailMessage(energyConsumptionSettings.DepotId, chargePointId, DateTime.UtcNow, totalEnergyConsumedByChargePoint, chargePointLimit);
            await _emailService.SendMessageAsync(warningEmailMessage, cancellationToken: cancellationToken);

            var energyLimitExceededMessage = new EnergyLimitExceededMessage
            {
                ChargePointId = chargePointId,
                DepotId = energyConsumptionSettings.DepotId,
                EnergyConsumptionLimit = chargePointLimit,
                EnergyConsumption = totalEnergyConsumedByChargePoint,
                WarningTimestamp = DateTime.UtcNow
            };

            var energyLimitExceededSignalRMessage = new SignalRMessage(JsonConvert.SerializeObject(energyLimitExceededMessage), nameof(EnergyLimitExceededMessage));
            await _publishEndpoint.Publish(energyLimitExceededSignalRMessage, cancellationToken);
        }
    }
}