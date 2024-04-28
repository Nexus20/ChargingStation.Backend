using System.Globalization;
using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using ChargingStation.InternalCommunication.Services.Connectors;
using ChargingStation.InternalCommunication.Services.EnergyConsumption;
using ChargingStation.InternalCommunication.SignalRModels;
using ChargingStation.Mailing.Messages;
using ChargingStation.Mailing.Services;
using ChargingStation.Transactions.Models.Requests;
using ChargingStation.Transactions.Repositories.ConnectorMeterValues;
using ChargingStation.Transactions.Specifications;
using MassTransit;
using Newtonsoft.Json;

namespace ChargingStation.Transactions.Services.MeterValues;

public class MeterValueService : IMeterValueService
{
    private readonly IConnectorHttpService _connectorHttpService;
    private readonly IEnergyConsumptionHttpService _energyConsumptionHttpService;
    private readonly IRepository<OcppTransaction> _transactionRepository;
    private readonly IConnectorMeterValueRepository _connectorMeterValueRepository;
    private readonly ILogger<MeterValueService> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IEmailService _emailService;

    public MeterValueService(ILogger<MeterValueService> logger, IConnectorHttpService connectorHttpService, IRepository<OcppTransaction> transactionRepository, IConnectorMeterValueRepository connectorMeterValueRepository, IPublishEndpoint publishEndpoint, IEnergyConsumptionHttpService energyConsumptionHttpService, IEmailService emailService)
    {
        _logger = logger;
        _connectorHttpService = connectorHttpService;
        _transactionRepository = transactionRepository;
        _connectorMeterValueRepository = connectorMeterValueRepository;
        _publishEndpoint = publishEndpoint;
        _energyConsumptionHttpService = energyConsumptionHttpService;
        _emailService = emailService;
    }

    public async Task<MeterValuesResponse> ProcessMeterValueAsync(MeterValuesRequest request, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var response = new MeterValuesResponse();
        var connectorChangesMessage = new ConnectorChangesMessage();

        var connectorId = -1;
        var msgMeterValue = string.Empty;

        try
        {
            connectorId = request.ConnectorId;

            var connectorRequest = new GetOrCreateConnectorRequest
            {
                ChargePointId = chargePointId,
                ConnectorId = connectorId
            };
            // TODO: update to just get
            var connector = await _connectorHttpService.GetOrCreateConnectorAsync(connectorRequest, cancellationToken);
            
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
            DateTimeOffset? meterTime = null;
            foreach (var meterValue in request.MeterValue)
            {
                foreach (var sampleValue in meterValue.SampledValue)
                {
                    _logger.LogTrace("MeterValues => Context={Context} / Format={Format} / Value={Value} / Unit={Unit} / Location={Location} / Measurand={Measurand} / Phase={Phase}",
                        sampleValue.Context, sampleValue.Format, sampleValue.Value, sampleValue.Unit, sampleValue.Location, sampleValue.Measurand, sampleValue.Phase);

                    if (sampleValue.Measurand == SampledValueMeasurand.Power_Active_Import)
                    {
                        // current charging power
                        if (double.TryParse(sampleValue.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var currentChargeKw))
                        {
                            if (sampleValue.Unit is SampledValueUnit.W or SampledValueUnit.VA or SampledValueUnit.Var or null)
                            {
                                connectorChangesMessage.Energy = currentChargeKw;
                                _logger.LogTrace("MeterValues => Charging '{CurrentChargeW:0.0}' W", currentChargeKw);
                                // convert W => kW
                                currentChargeKw = currentChargeKw / 1000;
                            }
                            else if (sampleValue.Unit is SampledValueUnit.KW or SampledValueUnit.KVA or SampledValueUnit.Kvar)
                            {
                                // already kW => OK
                                _logger.LogTrace("MeterValues => Charging '{CurrentChargeKw:0.0}' kW", currentChargeKw);
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
                        if (double.TryParse(sampleValue.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var meterKwh))
                        {
                            if (sampleValue.Unit is SampledValueUnit.Wh or SampledValueUnit.Varh or null)
                            {
                                _logger.LogTrace("MeterValues => Value: '{MeterWh:0.0}' Wh", meterKwh);
                                // convert Wh => kWh
                                meterKwh = meterKwh / 1000;
                            }
                            else if (sampleValue.Unit is SampledValueUnit.KWh or SampledValueUnit.Kvarh)
                            {
                                // already kWh => OK
                                _logger.LogTrace("MeterValues => Value: '{MeterKwh:0.0}' kWh", meterKwh);
                            }
                            else
                            {
                                _logger.LogWarning("MeterValues => Value: unexpected unit: '{Unit}' (Value={Value})", sampleValue.Unit, sampleValue.Value);
                            }
                            meterTime = meterValue.Timestamp;
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
                            connectorChangesMessage.SoC = (int)stateOfCharge;
                            _logger.LogTrace("MeterValues => SoC: '{0:0.0}'%", stateOfCharge);
                        }
                        else
                        {
                            _logger.LogError("MeterValues => invalid value '{Value}' (SoC)", sampleValue.Value);
                        }
                    }
                    
                    var meterValueToAdd = new ConnectorMeterValue
                    {
                        ConnectorId = connector.Id,
                        TransactionId = transaction.Id,
                        Value = sampleValue.Value,
                        Measurand = sampleValue.Measurand!.Value.ToString(),
                        Location = sampleValue.Location.ToString(),
                        Phase = sampleValue.Phase.ToString(),
                        Format = sampleValue.Format.ToString(),
                        Unit = sampleValue.Unit.ToString(),
                        MeterValueTimestamp = meterTime?.UtcDateTime ?? DateTime.UtcNow
                    };
                    meterValuesToAdd.Add(meterValueToAdd);
                }
                await _connectorMeterValueRepository.AddRangeAsync(meterValuesToAdd, cancellationToken);
                await _connectorMeterValueRepository.SaveChangesAsync(cancellationToken);

                connectorChangesMessage.ChargePointId = chargePointId;
                connectorChangesMessage.ConnectorId = connector.Id;
                connectorChangesMessage.TransactionId = transaction.TransactionId;

                var signalRMessage = new SignalRMessage(JsonConvert.SerializeObject(connectorChangesMessage), nameof(connectorChangesMessage));
                await _publishEndpoint.Publish(signalRMessage, cancellationToken);
                
                await CheckEnergyConsumptionLimitAndWarnAsync(chargePointId, cancellationToken);
            }
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "MeterValues => Exception: {Message}", exp.Message);
        }

        return response;
    }

    private async Task CheckEnergyConsumptionLimitAndWarnAsync(Guid chargePointId,
        CancellationToken cancellationToken = default)
    {
        var energyConsumptionSettings =
            await _energyConsumptionHttpService.GetByChargingStationIdAsync(chargePointId, cancellationToken);

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