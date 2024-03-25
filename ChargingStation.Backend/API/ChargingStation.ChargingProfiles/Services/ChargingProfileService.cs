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
using ChargingStation.InternalCommunication.Services.Connectors;
using ChargingStation.InternalCommunication.Services.Transactions;
using MassTransit;
using ChargingProfile = ChargingStation.Domain.Entities.ChargingProfile;
using ChargingScheduleChargingRateUnit = ChargingStation.Common.Messages_OCPP16.Responses.Enums.ChargingScheduleChargingRateUnit;
using ChargingSchedulePeriod = ChargingStation.Common.Messages_OCPP16.Responses.Enums.ChargingSchedulePeriod;
using SetChargingProfileRequest = ChargingStation.Common.Messages_OCPP16.Requests.SetChargingProfileRequest;

namespace ChargingStation.ChargingProfiles.Services;

public class ChargingProfileService : IChargingProfileService
{
    private readonly ILogger<ChargingProfileService> _logger;
    private readonly IRepository<ConnectorChargingProfile> _connectorChargingProfileRepository;
    private readonly IRepository<ChargingProfile> _chargingProfileRepository;
    private readonly IConnectorHttpService _connectorHttpService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICacheManager _cacheManager;
    private readonly ITransactionHttpService _transactionHttpService;
    private readonly IMapper _mapper;

    public ChargingProfileService(ILogger<ChargingProfileService> logger, IRepository<ConnectorChargingProfile> connectorChargingProfileRepository, IRepository<ChargingProfile> chargingProfileRepository, IPublishEndpoint publishEndpoint, IConnectorHttpService connectorHttpService, ICacheManager cacheManager, ITransactionHttpService transactionHttpService, IMapper mapper)
    {
        _logger = logger;
        _connectorChargingProfileRepository = connectorChargingProfileRepository;
        _chargingProfileRepository = chargingProfileRepository;
        _publishEndpoint = publishEndpoint;
        _connectorHttpService = connectorHttpService;
        _cacheManager = cacheManager;
        _transactionHttpService = transactionHttpService;
        _mapper = mapper;
    }

    public async Task ProcessSetChargingProfileResponseAsync(SetChargingProfileResponse setChargingProfileResponse,
        Guid chargePointId, string ocppMessageId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pendingSetChargingProfileRequest = await _cacheManager.GetAsync<SetChargingProfileRequest>(ocppMessageId);
                
            if (pendingSetChargingProfileRequest == null)
            {
                _logger.LogError("Pending set charging profile request not found for unique id {UniqueId}", ocppMessageId);
                return;
            }
            
            switch (setChargingProfileResponse.Status)
            {
                case SetChargingProfileResponseStatus.Accepted:
                {
                    var connector = await _connectorHttpService.GetAsync(chargePointId, pendingSetChargingProfileRequest.ConnectorId, cancellationToken);
                
                    if (connector == null)
                        throw new NotFoundException($"Connector of charge point {chargePointId} with id {pendingSetChargingProfileRequest.ConnectorId} not found");
                
                    var specification = new GetChargingProfileByNumericIdSpecification(pendingSetChargingProfileRequest.CsChargingProfiles.ChargingProfileId);
                
                    var chargingProfile = await _chargingProfileRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
                
                    if (chargingProfile == null)
                        throw new NotFoundException(nameof(ChargingProfile), pendingSetChargingProfileRequest.CsChargingProfiles.ChargingProfileId);
                
                    var connectorChargingProfile = new ConnectorChargingProfile
                    {
                        ConnectorId = connector.Id,
                        ChargingProfileId = chargingProfile.Id,
                    };
                
                    await _connectorChargingProfileRepository.AddAsync(connectorChargingProfile, cancellationToken);
                    await _connectorChargingProfileRepository.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Connector charging profile added for connector {ConnectorId} and charging profile {ChargingProfileId}", connector.Id, chargingProfile.Id);
                    return;
                }
                case SetChargingProfileResponseStatus.Rejected:
                    _logger.LogWarning("Set charging profile request rejected for charging profile {ChargingProfileId} and charge point {ChargePointId} with connector {ConnectorId}",
                        pendingSetChargingProfileRequest.CsChargingProfiles.ChargingProfileId, chargePointId, pendingSetChargingProfileRequest.ConnectorId);
                    return;
                case SetChargingProfileResponseStatus.NotSupported:
                    _logger.LogWarning("Charging profile {ChargingProfileId} is not supported on charge point {ChargePointId} with connector {ConnectorId}",
                        pendingSetChargingProfileRequest.CsChargingProfiles.ChargingProfileId, chargePointId, pendingSetChargingProfileRequest.ConnectorId);
                    return;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing set charging profile response message");
        }
    }

    public async Task SetChargingProfileAsync(Models.Requests.SetChargingProfileRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetChargingProfileWithSchedulesSpecification(request.ChargingProfileId);
        
        var chargingProfile = await _chargingProfileRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        if (chargingProfile == null)
        {
            throw new NotFoundException(nameof(ChargingProfile), request.ChargingProfileId);
        }
        
        var connector = await _connectorHttpService.GetByIdAsync(request.ConnectorId, cancellationToken);
        
        if (connector == null)
        {
            throw new NotFoundException(nameof(Connector), request.ConnectorId);
        }

        var chargingProfileKind = Enum.Parse<CsChargingProfilesChargingProfileKind>(chargingProfile.ChargingProfileKind.ToString());
        var chargingProfilePurpose = Enum.Parse<CsChargingProfilesChargingProfilePurpose>(chargingProfile.ChargingProfilePurpose.ToString());

        var chargingRateUnit = Enum.Parse<ChargingScheduleChargingRateUnit>(chargingProfile.SchedulingUnit.ToString());
        var ocppChargingSchedulePeriods = chargingProfile.ChargingSchedulePeriods.Select(x => new ChargingSchedulePeriod(x.Limit, x.NumberPhases, x.StartPeriod)).ToList();
        var ocppChargingSchedule = new ChargingSchedule(chargingRateUnit, ocppChargingSchedulePeriods);
        var ocppChargingProfile = new CsChargingProfiles(ocppChargingSchedule, chargingProfile.ChargingProfileId, chargingProfile.StackLevel, chargingProfilePurpose, chargingProfileKind);
        
        if (request.TransactionId.HasValue)
        {
            var transaction = await _transactionHttpService.GetByIdAsync(request.TransactionId.Value, cancellationToken);
            
            if (transaction == null)
                throw new NotFoundException(nameof(OcppTransaction), request.TransactionId.Value);
            
            ocppChargingProfile = ocppChargingProfile with { TransactionId = transaction.TransactionId };
        }
        
        var setChargingProfileRequest = new SetChargingProfileRequest(connector.ConnectorId, ocppChargingProfile);
        var uniqueId = Guid.NewGuid().ToString();
        var integrationOcppMessage = new IntegrationOcppMessage<SetChargingProfileRequest>(connector.ChargePointId, setChargingProfileRequest, uniqueId, OcppProtocolVersions.Ocpp16);
        
        await _cacheManager.SetAsync(uniqueId, setChargingProfileRequest);
        
        await _publishEndpoint.Publish(integrationOcppMessage, cancellationToken);
        _logger.LogInformation("Set charging profile request sent to charge point {ChargePointId} and connector {ConnectorId} with unique id {UniqueId}", connector.ChargePointId, connector.ConnectorId, uniqueId);
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
        var chargingProfile = _mapper.Map<ChargingProfile>(request);
        
        await _chargingProfileRepository.AddAsync(chargingProfile, cancellationToken);
        await _chargingProfileRepository.SaveChangesAsync(cancellationToken);
        
        var specification = new GetChargingProfileWithSchedulesSpecification(chargingProfile.Id);
        chargingProfile = await _chargingProfileRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);
        
        var result = _mapper.Map<ChargingProfileResponse>(chargingProfile);
        return result;
    }
}