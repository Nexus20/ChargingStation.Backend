using AutoMapper;
using ChargePointEmulator.Application.Interfaces;
using ChargePointEmulator.Application.Models;
using ChargePointEmulator.Application.Specifications;
using ChargePointEmulator.Application.State;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace ChargePointEmulator.Application.Services;

public class ChargePointService : IChargePointService
{
    private readonly IRepository<ChargePoint> _chargePointRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ChargePointService> _logger;
    private readonly IChargingStationStateRepository _chargingStationStateRepository;
    private readonly ChargingStationSimulatorManager _chargingStationSimulatorManager;

    public ChargePointService(IRepository<ChargePoint> chargePointRepository, IMapper mapper, ILogger<ChargePointService> logger, IChargingStationStateRepository chargingStationStateRepository, ChargingStationSimulatorManager chargingStationSimulatorManager)
    {
        _chargePointRepository = chargePointRepository;
        _mapper = mapper;
        _logger = logger;
        _chargingStationStateRepository = chargingStationStateRepository;
        _chargingStationSimulatorManager = chargingStationSimulatorManager;
    }

    public async Task<IPagedCollection<ChargePointSimulatorResponse>> GetAsync(GetChargePointRequest request,
        CancellationToken cancellationToken = default)
    {
        var specification = new GetChargePointsSpecification(request);

        var chargePoints = await _chargePointRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);

        if (!chargePoints.Collection.Any())
            return PagedCollection<ChargePointSimulatorResponse>.Empty;

        var result = _mapper.Map<IPagedCollection<ChargePointSimulatorResponse>>(chargePoints);
        
        var states = await _chargingStationStateRepository.GetAllAsync(cancellationToken);
        
        foreach (var chargePoint in result.Collection)
        {
            var state = states.FirstOrDefault(s => s.Id == chargePoint.Id.ToString());
            chargePoint.SimulatorAdded = state != null;
        }
        
        return result;
    }

    public async Task<ChargePointSimulatorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _chargePointRepository.GetByIdAsync(id, cancellationToken);

        if (chargePoint is null)
            throw new NotFoundException(nameof(ChargePoint), id);

        var result = _mapper.Map<ChargePointSimulatorResponse>(chargePoint);
        
        var state = await _chargingStationStateRepository.GetByIdAsync(id.ToString(), cancellationToken);

        if (state == null) 
            return result;
        
        result.SimulatorAdded = true;
        _chargingStationSimulatorManager.TryAddChargingStation(id);

        return result;
    }

    public async Task AddSimulatorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _chargingStationStateRepository.GetByIdAsync(id.ToString(), cancellationToken);
        
        if(chargePoint is not null)
            return;
        
        var chargingStationState = new ChargingStationState
        {
            Id = id.ToString(),
        };
        
        await _chargingStationStateRepository.InsertAsync(chargingStationState, cancellationToken);
    }
}