using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Helpers.OcppTags;
using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Models.Connectors.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.Reservations.Requests;
using ChargingStation.Common.Models.Transactions.Responses;
using ChargingStation.Domain.Entities;
using ChargingStation.InternalCommunication.Services.Connectors;
using ChargingStation.InternalCommunication.Services.OcppTags;
using ChargingStation.InternalCommunication.Services.Reservations;
using ChargingStation.InternalCommunication.SignalRModels;
using ChargingStation.Transactions.Models.Requests;
using ChargingStation.Transactions.Repositories.Transactions;
using ChargingStation.Transactions.Specifications;
using MassTransit;
using Newtonsoft.Json;

namespace ChargingStation.Transactions.Services.Transactions;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IOcppTagHttpService _ocppTagHttpService;
    private readonly IConnectorHttpService _connectorHttpService;
    private readonly IReservationHttpService _reservationHttpService;
    private readonly IMapper _mapper;
    private readonly ILogger<TransactionService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IPublishEndpoint _publishEndpoint;

    public TransactionService(ITransactionRepository transactionRepository, IMapper mapper,
        IOcppTagHttpService ocppTagHttpService, ILogger<TransactionService> logger, IConfiguration configuration, 
        IConnectorHttpService connectorHttpService, IReservationHttpService reservationHttpService,
        IPublishEndpoint publishEndpoint)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _ocppTagHttpService = ocppTagHttpService;
        _logger = logger;
        _configuration = configuration;
        _connectorHttpService = connectorHttpService;
        _reservationHttpService = reservationHttpService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<IPagedCollection<TransactionResponse>> GetAsync(GetTransactionsRequest request,
        CancellationToken cancellationToken = default)
    {
        var specification = new GetTransactionsSpecification(request);

        var transactions = await _transactionRepository.GetPagedCollectionAsync(specification,
            request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);

        if (!transactions.Collection.Any())
            return PagedCollection<TransactionResponse>.Empty;

        var result = _mapper.Map<IPagedCollection<TransactionResponse>>(transactions);
        return result;
    }

    public async Task<TransactionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);

        if (transaction is null)
            throw new NotFoundException(nameof(OcppTransaction), id);

        var result = _mapper.Map<TransactionResponse>(transaction);
        return result;
    }

    public async Task<TransactionResponse> CreateAsync(CreateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        var transactionToCreate = _mapper.Map<OcppTransaction>(request);

        await _transactionRepository.AddAsync(transactionToCreate, cancellationToken);
        await _transactionRepository.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<TransactionResponse>(transactionToCreate);
        return result;
    }

    public async Task<TransactionResponse> UpdateAsync(UpdateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        var transactionToUpdate = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (transactionToUpdate is null)
            throw new NotFoundException(nameof(OcppTransaction), request.Id);

        _mapper.Map(request, transactionToUpdate);
        _transactionRepository.Update(transactionToUpdate);
        await _transactionRepository.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<TransactionResponse>(transactionToUpdate);
        return result;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transactionToRemove = await _transactionRepository.GetByIdAsync(id, cancellationToken);

        if (transactionToRemove is null)
            throw new NotFoundException(nameof(OcppTransaction), id);

        _transactionRepository.Remove(transactionToRemove);
        await _transactionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<StartTransactionResponse> ProcessStartTransactionAsync(StartTransactionRequest request, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var response = new StartTransactionResponse();

        var idTag = OcppTagHelper.CleanChargeTagId(request.IdTag, _logger);
        var connectorId = request.ConnectorId;

        var denyConcurrentTx = _configuration.GetValue("DenyConcurrentTx", false);

        response.IdTagInfo.ParentIdTag = string.Empty;
        response.IdTagInfo.ExpiryDate = new DateTime(2199, 12, 31);

        if (string.IsNullOrWhiteSpace(idTag))
        {
            // no RFID-Tag => accept request
            response.IdTagInfo.Status = IdTagInfoStatus.Accepted;
            _logger.LogInformation("StartTransaction => no charge tag => Status: {IdTagStatus}",
                response.IdTagInfo.Status);
        }
        else
        {
            try
            {
                var ocppTag = await _ocppTagHttpService.GetByOcppTagIdAsync(idTag, cancellationToken);

                if (ocppTag is null)
                {
                    response.IdTagInfo.Status = IdTagInfoStatus.Invalid;
                }
                else
                {
                    if (ocppTag.ExpiryDate.HasValue)
                        response.IdTagInfo.ExpiryDate = ocppTag.ExpiryDate.Value;

                    response.IdTagInfo.ParentIdTag = ocppTag.ParentTagId;

                    if (ocppTag.Blocked.HasValue && ocppTag.Blocked.Value)
                    {
                        response.IdTagInfo.Status = IdTagInfoStatus.Blocked;
                    }
                    else if (ocppTag.ExpiryDate.HasValue && ocppTag.ExpiryDate.Value < DateTime.Now)
                    {
                        response.IdTagInfo.Status = IdTagInfoStatus.Expired;
                    }
                    else
                    {
                        response.IdTagInfo.Status = IdTagInfoStatus.Accepted;

                        if (denyConcurrentTx)
                        {
                            var activeTransaction = await _transactionRepository.GetLastActiveTransactionAsync(idTag, cancellationToken);

                            if (activeTransaction is not null)
                            {
                                response.IdTagInfo.Status = IdTagInfoStatus.ConcurrentTx;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, "StartTransaction => Exception reading charge tag ({IdTag}): {Message}", idTag, exp.Message);
                response.IdTagInfo.Status = IdTagInfoStatus.Invalid;
                return response;
            }
        }

        if (response.IdTagInfo.Status == IdTagInfoStatus.Accepted)
        {
            try
            {
                var connectorRequest = new GetOrCreateConnectorRequest()
                {
                    ChargePointId = chargePointId,
                    ConnectorId = connectorId
                };
                var connector = await _connectorHttpService.GetOrCreateConnectorAsync(connectorRequest, cancellationToken);
                
                var updateStatusRequest = new UpdateConnectorStatusRequest()
                {
                    ChargePointId = chargePointId,
                    ConnectorId = connectorId,
                    Status = "Occupied",
                    StatusTimestamp = request.Timestamp.UtcDateTime,
                };
        
                await _connectorHttpService.UpdateConnectorStatusAsync(updateStatusRequest, cancellationToken);
                
                var transactionToCreate = new OcppTransaction
                {
                    ConnectorId = connector.Id,
                    StartTagId = idTag,
                    StartTime = request.Timestamp.UtcDateTime,
                    MeterStart = (double)request.MeterStart / 1000, // Meter value here is always Wh
                    StartResult = response.IdTagInfo.Status.ToString(),
                };

                await _transactionRepository.AddAsync(transactionToCreate, cancellationToken);
                await _transactionRepository.SaveChangesAsync(cancellationToken);

                var transactionMessage = new TransactionMessage()
                {
                    ChargePointId = chargePointId,
                    ConnectorId = connector.Id,
                    TransactionId = transactionToCreate.TransactionId
                };
                var signalRMessage = new SignalRMessage( JsonConvert.SerializeObject(transactionMessage), nameof(transactionMessage));
                await _publishEndpoint.Publish(signalRMessage, cancellationToken);

                // Return DB-ID as transaction ID
                response.TransactionId = transactionToCreate.TransactionId;
                
                if (request.ReservationId.HasValue)
                {
                    var useReservationRequest = new UseReservationRequest
                    {
                        ChargePointId = chargePointId,
                        ConnectorId = connector.Id,
                        ReservationId = request.ReservationId.Value
                    };
                    
                    await _reservationHttpService.UseReservationAsync(useReservationRequest, cancellationToken);
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp,
                    "StartTransaction => Exception writing transaction: charge point = {ChargePointId} / tag={IdTag}",
                    chargePointId, idTag);
            }
        }

        return response;
    }

    public async Task<StopTransactionResponse> ProcessStopTransactionAsync(StopTransactionRequest request,
        Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var response = new StopTransactionResponse();

        var idTag = OcppTagHelper.CleanChargeTagId(request.IdTag, _logger);

        if (string.IsNullOrWhiteSpace(idTag))
        {
            response.IdTagInfo.Status = IdTagInfoStatus.Accepted;
            _logger.LogInformation("StopTransaction => no charge tag => Status: {IdTagStatus}",
                response.IdTagInfo.Status);
        }
        else
        {
            response.IdTagInfo = new IdTagInfo
            {
                ExpiryDate = new DateTime(2199, 12, 31)
            };

            try
            {
                var ocppTag = await _ocppTagHttpService.GetByOcppTagIdAsync(idTag, cancellationToken);

                if (ocppTag != null)
                {
                    if (ocppTag.ExpiryDate.HasValue) response.IdTagInfo.ExpiryDate = ocppTag.ExpiryDate.Value;
                    response.IdTagInfo.ParentIdTag = ocppTag.ParentTagId;
                    if (ocppTag.Blocked.HasValue && ocppTag.Blocked.Value)
                    {
                        response.IdTagInfo.Status = IdTagInfoStatus.Blocked;
                    }
                    else if (ocppTag.ExpiryDate.HasValue && ocppTag.ExpiryDate.Value < DateTime.Now)
                    {
                        response.IdTagInfo.Status = IdTagInfoStatus.Expired;
                    }
                    else
                    {
                        response.IdTagInfo.Status = IdTagInfoStatus.Accepted;
                    }
                }
                else
                {
                    response.IdTagInfo.Status = IdTagInfoStatus.Invalid;
                }

                _logger.LogInformation("StopTransaction => RFID-tag='{0}' => Status: {1}", idTag,
                    response.IdTagInfo.Status);
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, "StopTransaction => Exception reading charge tag ({IdTag}): {Message}", idTag, exp.Message);
                response.IdTagInfo.Status = IdTagInfoStatus.Invalid;
            }
        }

        if (response.IdTagInfo.Status == IdTagInfoStatus.Accepted)
        {
            try
            {
                var getTransactionRequest = new GetTransactionsRequest
                {
                    TransactionId = request.TransactionId
                };
                
                var specification = new GetTransactionsSpecification(getTransactionRequest);
                
                var transaction = await _transactionRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);

                if (transaction is not null)
                {
                    var connector = await _connectorHttpService.GetByIdAsync(transaction.ConnectorId, cancellationToken);
                    
                    if (connector.ChargePointId == chargePointId && !transaction.StopTime.HasValue) {
                        
                        var updateStatusRequest = new UpdateConnectorStatusRequest()
                        {
                            ChargePointId = chargePointId,
                            ConnectorId = connector.ConnectorId,
                            Status = "Available",
                            StatusTimestamp = request.Timestamp.UtcDateTime,
                        };
                        await _connectorHttpService.UpdateConnectorStatusAsync(updateStatusRequest, cancellationToken);

                        // check current tag against start tag
                        bool valid = true;
                        if (!string.Equals(transaction.StartTagId, idTag, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // tags are different => same group?
                            var startTag =
                                await _ocppTagHttpService.GetByOcppTagIdAsync(transaction.StartTagId,
                                    cancellationToken);

                            if (startTag != null)
                            {
                                if (!string.Equals(startTag.ParentTagId, response.IdTagInfo.ParentIdTag,
                                        StringComparison.InvariantCultureIgnoreCase))
                                {
                                    _logger.LogInformation(
                                        "StopTransaction => Start-Tag ('{StartIdTag}') and End-Tag ('{EndIdTag}') do not match: Invalid!",
                                        transaction.StartTagId, idTag);

                                    response.IdTagInfo.Status = IdTagInfoStatus.Invalid;
                                    valid = false;
                                }
                                else
                                {
                                    _logger.LogInformation(
                                        "StopTransaction => Different RFID-Tags but matching group ('{ParentIdTag}')",
                                        response.IdTagInfo.ParentIdTag);
                                }
                            }
                            else
                            {
                                _logger.LogError("StopTransaction => Start-Tag not found: '{IdTag}'",
                                    transaction.StartTagId);
                                // assume "valid" and allow to end the transaction
                            }
                        }

                        if (valid)
                        {
                            transaction.StopTagId = idTag;
                            transaction.MeterStop = (double)request.MeterStop / 1000; // Meter value here is always Wh
                            transaction.StopReason = request.Reason.ToString();
                            transaction.StopTime = request.Timestamp.UtcDateTime;

                            _transactionRepository.Update(transaction);
                            await _transactionRepository.SaveChangesAsync(cancellationToken);

                            var transactionMessage = new TransactionMessage()
                            {
                                ChargePointId = chargePointId,
                                ConnectorId = transaction.ConnectorId,
                                TransactionId = transaction.TransactionId,
                            };
                            var signalRMessage = new SignalRMessage(JsonConvert.SerializeObject(transactionMessage), nameof(transactionMessage));
                            await _publishEndpoint.Publish(signalRMessage, cancellationToken);
                        }
                        
                        await ChangeAvailabilityIfEnergyConsumptionLimitReachedAsync(chargePointId, cancellationToken);
                    }
                }
                else
                {
                    _logger.LogError("StopTransaction => Unknown or not matching transaction: id={TransactionId} / chargepoint={ChargePointId} / tag={IdTag}", request.TransactionId, chargePointId, idTag);
                }
                
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, "StopTransaction => Exception writing transaction: chargepoint={ChargePointId} / tag={IdTag}", chargePointId, idTag);
            }
        }

        return response;
    }

    private async Task ChangeAvailabilityIfEnergyConsumptionLimitReachedAsync(Guid chargePointId, CancellationToken cancellationToken)
    {
        
    }
}