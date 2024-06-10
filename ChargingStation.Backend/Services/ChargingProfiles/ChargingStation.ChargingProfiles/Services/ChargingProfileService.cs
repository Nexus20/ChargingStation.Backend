using System.Text;
using AutoMapper;
using ChargingStation.CacheManager;
using ChargingStation.ChargingProfiles.Models.Requests;
using ChargingStation.ChargingProfiles.Models.Responses;
using ChargingStation.ChargingProfiles.Specifications;
using ChargingStation.Common.Constants;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using ChargingStation.InternalCommunication.GrpcClients;
using ChargingStation.InternalCommunication.SignalRModels;
using MassTransit;
using Newtonsoft.Json;
using ChargingProfile = ChargingStation.Domain.Entities.ChargingProfile;
using ChargingScheduleChargingRateUnit = ChargingStation.Common.Messages_OCPP16.Responses.Enums.ChargingScheduleChargingRateUnit;
using ChargingSchedulePeriod = ChargingStation.Common.Messages_OCPP16.Responses.Enums.ChargingSchedulePeriod;
using SetChargingProfileOcppRequest = ChargingStation.Common.Messages_OCPP16.Requests.SetChargingProfileRequest;
using ClearChargingProfileOcppRequest = ChargingStation.Common.Messages_OCPP16.Requests.ClearChargingProfileRequest;

namespace ChargingStation.ChargingProfiles.Services;

public class ChargingProfileService : IChargingProfileService
{
    private readonly ILogger<ChargingProfileService> _logger;
    private readonly IRepository<ConnectorChargingProfile> _connectorChargingProfileRepository;
    private readonly IRepository<ChargingProfile> _chargingProfileRepository;
    private readonly ConnectorGrpcClientService _connectorGrpcClientService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICacheManager _cacheManager;
    private readonly TransactionGrpcClientService _transactionGrpcClientService;
    private readonly IMapper _mapper;

    public ChargingProfileService(ILogger<ChargingProfileService> logger, IRepository<ConnectorChargingProfile> connectorChargingProfileRepository, IRepository<ChargingProfile> chargingProfileRepository, IPublishEndpoint publishEndpoint, ConnectorGrpcClientService connectorGrpcClientService, ICacheManager cacheManager, TransactionGrpcClientService transactionGrpcClientService, IMapper mapper)
    {
        _logger = logger;
        _connectorChargingProfileRepository = connectorChargingProfileRepository;
        _chargingProfileRepository = chargingProfileRepository;
        _publishEndpoint = publishEndpoint;
        _connectorGrpcClientService = connectorGrpcClientService;
        _cacheManager = cacheManager;
        _transactionGrpcClientService = transactionGrpcClientService;
        _mapper = mapper;
    }

    public async Task ProcessSetChargingProfileResponseAsync(SetChargingProfileResponse setChargingProfileResponse,
        Guid chargePointId, string ocppMessageId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pendingSetChargingProfileRequest = await _cacheManager.GetAsync<SetChargingProfileOcppRequest>(ocppMessageId);
                
            if (pendingSetChargingProfileRequest == null)
            {
                _logger.LogError("Pending set charging profile request not found for unique id {UniqueId}", ocppMessageId);
                return;
            }
            
            var connector = await _connectorGrpcClientService.GetByChargePointIdAsync(chargePointId, pendingSetChargingProfileRequest.ConnectorId, cancellationToken);
                
            if (connector == null)
                throw new NotFoundException($"Connector of charge point {chargePointId} with id {pendingSetChargingProfileRequest.ConnectorId} not found");
                
            var specification = new GetChargingProfileByNumericIdSpecification(pendingSetChargingProfileRequest.CsChargingProfiles.ChargingProfileId);
                
            var chargingProfile = await _chargingProfileRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
                
            if (chargingProfile == null)
                throw new NotFoundException(nameof(ChargingProfile), pendingSetChargingProfileRequest.CsChargingProfiles.ChargingProfileId);
            
            switch (setChargingProfileResponse.Status)
            {
                case SetChargingProfileResponseStatus.Accepted:
                {
                    var connectorChargingProfile = new ConnectorChargingProfile
                    {
                        ConnectorId = connector.Id,
                        ChargingProfileId = chargingProfile.Id,
                    };
                
                    await _connectorChargingProfileRepository.AddAsync(connectorChargingProfile, cancellationToken);
                    await _connectorChargingProfileRepository.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Connector charging profile added for connector {ConnectorId} and charging profile {ChargingProfileId}", connector.Id, chargingProfile.Id);
                    break;
                }
                case SetChargingProfileResponseStatus.Rejected:
                    _logger.LogWarning("Set charging profile request rejected for charging profile {ChargingProfileId} and charge point {ChargePointId} with connector {ConnectorId}",
                        pendingSetChargingProfileRequest.CsChargingProfiles.ChargingProfileId, chargePointId, pendingSetChargingProfileRequest.ConnectorId);
                    break;
                case SetChargingProfileResponseStatus.NotSupported:
                    _logger.LogWarning("Charging profile {ChargingProfileId} is not supported on charge point {ChargePointId} with connector {ConnectorId}",
                        pendingSetChargingProfileRequest.CsChargingProfiles.ChargingProfileId, chargePointId, pendingSetChargingProfileRequest.ConnectorId);
                    break;
            }
            
            var chargingProfileSetMessage = new ChargingProfileSetMessage
            {
                ChargePointId = chargePointId,
                ConnectorId = pendingSetChargingProfileRequest.ConnectorId,
                Status = setChargingProfileResponse.Status,
                ChargingProfileId = chargingProfile.Id,
            };

            var chargingProfileSetSignalRMessage = new SignalRMessage(JsonConvert.SerializeObject(chargingProfileSetMessage), nameof(ChargingProfileSetMessage));
            await _publishEndpoint.Publish(chargingProfileSetSignalRMessage, cancellationToken);
            
            await _cacheManager.RemoveAsync(ocppMessageId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing set charging profile response message");
        }
    }

    public async Task ClearChargingProfileAsync(ClearChargingProfileRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetChargingProfileWithConnectorSpecification(request.ChargingProfileId);
        
        var chargingProfile = await _chargingProfileRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (chargingProfile == null)
            throw new NotFoundException(nameof(ChargingProfile), request.ChargingProfileId);
        
        if(chargingProfile.ConnectorChargingProfiles is null or { Count: 0 })
            throw new BadRequestException($"Charging profile {request.ChargingProfileId} does not have any connectors assigned");
        
        if(chargingProfile.ConnectorChargingProfiles.All(x => x.ConnectorId != request.ConnectorId))
            throw new BadRequestException($"Connector {request.ConnectorId} is not assigned to charging profile {request.ChargingProfileId}");
        
        var connector = await _connectorGrpcClientService.GetByIdAsync(request.ConnectorId, cancellationToken);
        
        var clearChargingProfileRequest = new ClearChargingProfileOcppRequest
        {
            Id = chargingProfile.ChargingProfileId,
            ConnectorId = connector.ConnectorId,
        };
        
        var clearChargingProfileRequestId = Guid.NewGuid().ToString();
        var integrationOcppMessage = CentralSystemRequestIntegrationOcppMessage.Create(connector.ChargePointId, clearChargingProfileRequest, Ocpp16ActionTypes.ClearChargingProfile, clearChargingProfileRequestId, OcppProtocolVersions.Ocpp16);
        
        await _cacheManager.SetAsync(clearChargingProfileRequestId, clearChargingProfileRequest);
        
        await _publishEndpoint.Publish(integrationOcppMessage, cancellationToken);
        _logger.LogInformation("Clear charging profile request sent to charge point {ChargePointId} and connector {ConnectorId} with unique id {UniqueId}", connector.ChargePointId, connector.ConnectorId, clearChargingProfileRequestId);
    }

    public async Task ProcessClearChargingProfileResponseAsync(ClearChargingProfileResponse clearChargingProfileResponse,
        Guid chargePointId, string ocppMessageId, CancellationToken cancellationToken = default)
    {
        try
        {
            var pendingClearChargingProfileRequest = await _cacheManager.GetAsync<ClearChargingProfileOcppRequest>(ocppMessageId);
                
            if (pendingClearChargingProfileRequest == null)
            {
                _logger.LogError("Pending clear charging profile request not found for unique id {UniqueId}", ocppMessageId);
                return;
            }
            
            var connector = await _connectorGrpcClientService.GetByChargePointIdAsync(chargePointId, pendingClearChargingProfileRequest.ConnectorId.Value, cancellationToken);
                
            if (connector == null)
                throw new NotFoundException($"Connector of charge point {chargePointId} with id {pendingClearChargingProfileRequest.ConnectorId} not found");
            
            var specification = new GetChargingProfileByNumericIdSpecification(pendingClearChargingProfileRequest.Id.Value);
                
            var chargingProfile = await _chargingProfileRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
                
            if (chargingProfile == null)
                throw new NotFoundException(nameof(ChargingProfile), pendingClearChargingProfileRequest.Id);
            
            switch (clearChargingProfileResponse.Status)
            {
                case ClearChargingProfileResponseStatus.Accepted:
                {
                    // TODO: Add case handling for connector zero
                    // TODO: Add case for handling clearing profile for all connectors and by purpose or stack level
                
                    var connectorChargingProfileSpecification = new GetConnectorChargingProfileByConnectorAndProfileIdSpecification(connector.Id, chargingProfile.Id);
                    var connectorChargingProfile = await _connectorChargingProfileRepository.GetFirstOrDefaultAsync(connectorChargingProfileSpecification, cancellationToken: cancellationToken);
                    
                    if (connectorChargingProfile == null)
                        throw new NotFoundException($"Connector with id {connector.Id} does not have charging profile with id {chargingProfile.Id} assigned");
                    
                    _connectorChargingProfileRepository.Remove(connectorChargingProfile);
                    await _connectorChargingProfileRepository.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Charging profile {ChargingProfileId} cleared from connector {ConnectorId}", chargingProfile.Id, connector.Id);
                    break;
                }
                case ClearChargingProfileResponseStatus.Unknown:
                    _logger.LogWarning("Clear charging profile request for charging profile {ChargingProfileId} and charge point {ChargePointId} with connector {ConnectorId} returned unknown status because no charging profile found with given criteria",
                        pendingClearChargingProfileRequest.Id, chargePointId, pendingClearChargingProfileRequest.ConnectorId);
                    break;
            }
            
            var chargingProfileClearedMessage = new ChargingProfileClearedMessage
            {
                ChargePointId = chargePointId,
                ConnectorId = pendingClearChargingProfileRequest.ConnectorId,
                Status = clearChargingProfileResponse.Status,
                ChargingProfileId = chargingProfile.Id,
                StackLevel = pendingClearChargingProfileRequest.StackLevel,
                ChargingProfilePurpose = pendingClearChargingProfileRequest.ChargingProfilePurpose
            };

            var chargingProfileClearedSignalRMessage = new SignalRMessage(JsonConvert.SerializeObject(chargingProfileClearedMessage), nameof(ChargingProfileClearedMessage));
            await _publishEndpoint.Publish(chargingProfileClearedSignalRMessage, cancellationToken);
            
            await _cacheManager.RemoveAsync(ocppMessageId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing set charging profile response message");
        }
    }

    public async Task SetChargingProfileAsync(SetChargingProfileRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetChargingProfileWithSchedulesSpecification(request.ChargingProfileId);
        
        var chargingProfile = await _chargingProfileRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (chargingProfile == null)
            throw new NotFoundException(nameof(ChargingProfile), request.ChargingProfileId);
        
        if(chargingProfile.ValidTo < DateTime.UtcNow)
            throw new BadRequestException("Charging profile is not valid yet");
        
        var connector = await _connectorGrpcClientService.GetByIdAsync(request.ConnectorId, cancellationToken);
        
        if (connector == null)
            throw new NotFoundException(nameof(Connector), request.ConnectorId);
        
        if(connector.ConnectorId > 0 && chargingProfile.ChargingProfilePurpose == ChargingProfilePurpose.ChargePointMaxProfile)
            throw new BadRequestException("Connector number must be 0 for charging profile with purpose ChargePointMaxProfile");

        var chargingProfilePurpose = Enum.Parse<CsChargingProfilesChargingProfilePurpose>(chargingProfile.ChargingProfilePurpose.ToString());
        
        var sameStackLevelAndPurposeProfilesSpecification = new GetChargingProfileWithSchedulesByStackLevelAndPurposeSpecification(chargingProfile.StackLevel, chargingProfile.ChargingProfilePurpose);
        var sameStackLevelAndPurposeProfiles = await _chargingProfileRepository.GetAsync(sameStackLevelAndPurposeProfilesSpecification, cancellationToken: cancellationToken);

        if (sameStackLevelAndPurposeProfiles.Count > 0)
        {
            var getConnectorChargingProfilesSpecification = new GetConnectorChargingProfilesSpecification(connector.Id);
            var connectorChargingProfiles = await _connectorChargingProfileRepository.GetAsync(getConnectorChargingProfilesSpecification, cancellationToken: cancellationToken);
            
            var sameStackLevelAndPurposeProfilesIds = sameStackLevelAndPurposeProfiles.Select(x => x.Id).ToList();
            
            var conflictingProfiles = connectorChargingProfiles.Where(x => sameStackLevelAndPurposeProfilesIds.Contains(x.ChargingProfileId)).ToList();

            if (conflictingProfiles.Count > 0)
            {
                var exceptionMessageStringBuilder = new StringBuilder();
                exceptionMessageStringBuilder.AppendLine($"Connector {connector.Id} already has charging profile with same stack level {chargingProfile.StackLevel} and purpose {chargingProfile.ChargingProfilePurpose.ToString()}");
                exceptionMessageStringBuilder.AppendLine("Conflicting profiles:");
                
                foreach (var conflictingProfile in conflictingProfiles) 
                    exceptionMessageStringBuilder.AppendLine($"Charging profile {conflictingProfile.ChargingProfileId}");
                
                throw new BadRequestException(exceptionMessageStringBuilder.ToString());
            }
        }
        
        var chargingProfileKind = Enum.Parse<CsChargingProfilesChargingProfileKind>(chargingProfile.ChargingProfileKind.ToString());

        var chargingRateUnit = Enum.Parse<ChargingScheduleChargingRateUnit>(chargingProfile.SchedulingUnit.ToString());
        var ocppChargingSchedulePeriods = chargingProfile.ChargingSchedulePeriods.Select(x => new ChargingSchedulePeriod(x.Limit, x.NumberPhases, x.StartPeriod)).ToList();
        var ocppChargingSchedule = new ChargingSchedule(chargingRateUnit, ocppChargingSchedulePeriods);
        var ocppChargingProfile = new CsChargingProfiles(ocppChargingSchedule, chargingProfile.ChargingProfileId, chargingProfile.StackLevel, chargingProfilePurpose, chargingProfileKind);
        
        if (request.TransactionId.HasValue)
        {
            var transaction = await _transactionGrpcClientService.GetByIdAsync(request.TransactionId.Value, cancellationToken);
            
            if (transaction == null)
                throw new NotFoundException(nameof(OcppTransaction), request.TransactionId.Value);
            
            ocppChargingProfile = ocppChargingProfile with { TransactionId = transaction.TransactionId };
        }
        
        var setChargingProfileRequest = new SetChargingProfileOcppRequest(connector.ConnectorId, ocppChargingProfile);
        var setChargingProfileRequestId = Guid.NewGuid().ToString();
        var integrationOcppMessage = CentralSystemRequestIntegrationOcppMessage.Create(connector.ChargePointId, setChargingProfileRequest, Ocpp16ActionTypes.SetChargingProfile, setChargingProfileRequestId, OcppProtocolVersions.Ocpp16);
        
        await _cacheManager.SetAsync(setChargingProfileRequestId, setChargingProfileRequest);
        
        await _publishEndpoint.Publish(integrationOcppMessage, cancellationToken);
        _logger.LogInformation("Set charging profile request sent to charge point {ChargePointId} and connector {ConnectorId} with unique id {UniqueId}", connector.ChargePointId, connector.ConnectorId, setChargingProfileRequestId);
    }

    public async Task<IPagedCollection<ChargingProfileResponse>> GetAsync(GetChargingProfilesRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetChargingProfilesSpecification(request);
        
        var source = await _chargingProfileRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);
        
        if (!source.Collection.Any())
            return PagedCollection<ChargingProfileResponse>.Empty;
        
        var result = _mapper.Map<IPagedCollection<ChargingProfileResponse>>(source);
        return result;
    }

    public async Task<ChargingProfileResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var specification = new GetChargingProfileWithSchedulesSpecification(id);
        var chargingProfile = await _chargingProfileRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (chargingProfile == null)
            throw new NotFoundException(nameof(ChargingProfile), id);
        
        var result = _mapper.Map<ChargingProfileResponse>(chargingProfile);
        return result;
    }

    public async Task<ChargingProfileResponse> CreateAsync(CreateChargingProfileRequest request, CancellationToken cancellationToken = default)
    {
        if(request.ValidTo < DateTime.UtcNow)
            throw new BadRequestException("Valid to date must be in the future");
        
        var chargingProfile = _mapper.Map<ChargingProfile>(request);
        
        await _chargingProfileRepository.AddAsync(chargingProfile, cancellationToken);
        await _chargingProfileRepository.SaveChangesAsync(cancellationToken);
        
        var specification = new GetChargingProfileWithSchedulesSpecification(chargingProfile.Id);
        chargingProfile = await _chargingProfileRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        var result = _mapper.Map<ChargingProfileResponse>(chargingProfile);
        return result;
    }
}