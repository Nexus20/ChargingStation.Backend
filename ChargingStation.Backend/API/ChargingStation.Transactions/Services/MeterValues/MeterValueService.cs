using System.Globalization;
using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using ChargingStation.InternalCommunication.Services.Connectors;
using ChargingStation.InternalCommunication.SignalRModels;
using ChargingStation.Transactions.Models.Requests;
using ChargingStation.Transactions.Specifications;
using MassTransit;
using Newtonsoft.Json;

namespace ChargingStation.Transactions.Services.MeterValues;

public class MeterValueService : IMeterValueService
{
    private readonly IConnectorHttpService _connectorHttpService;
    private readonly IRepository<OcppTransaction> _transactionRepository;
    private readonly IRepository<ConnectorMeterValue> _connectorMeterValueRepository;
    private readonly ILogger<MeterValueService> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public MeterValueService(ILogger<MeterValueService> logger, IConnectorHttpService connectorHttpService, IRepository<OcppTransaction> transactionRepository, IRepository<ConnectorMeterValue> connectorMeterValueRepository, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _connectorHttpService = connectorHttpService;
        _transactionRepository = transactionRepository;
        _connectorMeterValueRepository = connectorMeterValueRepository;
        _publishEndpoint = publishEndpoint;
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
                                _logger.LogTrace("MeterValues => Charging '{0:0.0}' W", currentChargeKw);
                                // convert W => kW
                                currentChargeKw = currentChargeKw / 1000;
                            }
                            else if (sampleValue.Unit is SampledValueUnit.KW or SampledValueUnit.KVA or SampledValueUnit.Kvar)
                            {
                                // already kW => OK
                                _logger.LogTrace("MeterValues => Charging '{0:0.0}' kW", currentChargeKw);
                            }
                            else
                            {
                                _logger.LogWarning("MeterValues => Charging: unexpected unit: '{0}' (Value={1})", sampleValue.Unit, sampleValue.Value);
                            }
                        }
                        else
                        {
                            _logger.LogError("MeterValues => Charging: invalid value '{0}' (Unit={1})", sampleValue.Value, sampleValue.Unit);
                        }
                    }
                    else if (sampleValue.Measurand is SampledValueMeasurand.Energy_Active_Import_Register or null)
                    {
                        // charged amount of energy
                        if (double.TryParse(sampleValue.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var meterKwh))
                        {
                            if (sampleValue.Unit is SampledValueUnit.Wh or SampledValueUnit.Varh or null)
                            {
                                _logger.LogTrace("MeterValues => Value: '{0:0.0}' Wh", meterKwh);
                                // convert Wh => kWh
                                meterKwh = meterKwh / 1000;
                            }
                            else if (sampleValue.Unit is SampledValueUnit.KWh or SampledValueUnit.Kvarh)
                            {
                                // already kWh => OK
                                _logger.LogTrace("MeterValues => Value: '{0:0.0}' kWh", meterKwh);
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
                        Measurand = sampleValue.Measurand.ToString(),
                        Location = sampleValue.Location.ToString(),
                        Phase = sampleValue.Phase.ToString(),
                        Format = sampleValue.Format.ToString(),
                        Unit = sampleValue.Unit.ToString(),
                        MeterValueTimestamp = meterTime?.UtcDateTime,
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
            }
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "MeterValues => Exception: {Message}", exp.Message);
        }

        return response;
    }
}