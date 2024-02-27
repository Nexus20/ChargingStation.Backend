using AutoMapper;
using ChargingStation.ChargePoints.Models.Requests;
using ChargingStation.ChargePoints.Models.Responses;
using ChargingStation.ChargePoints.Specifications;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;

namespace ChargingStation.ChargePoints.Services;

public class ChargePointService : IChargePointService
{
    private readonly IRepository<ChargePoint> _chargePointRepository;
    private readonly IMapper _mapper;

    public ChargePointService(IRepository<ChargePoint> chargePointRepository, IMapper mapper)
    {
        _chargePointRepository = chargePointRepository;
        _mapper = mapper;
    }

    public async Task<IPagedCollection<ChargePointResponse>> GetAsync(GetChargePointRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetChargePointsSpecification(request);

        var chargePoints = await _chargePointRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);

        if (!chargePoints.Collection.Any())
            return PagedCollection<ChargePointResponse>.Empty;

        var result = _mapper.Map<IPagedCollection<ChargePointResponse>>(chargePoints);
        return result;
    }

    public async Task<ChargePointResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chargePoint = await _chargePointRepository.GetByIdAsync(id, cancellationToken);

        if (chargePoint is null)
            throw new NotFoundException(nameof(ChargePoint), id);

        var result = _mapper.Map<ChargePointResponse>(chargePoint);
        return result;
    }

    public async Task<ChargePointResponse> CreateAsync(CreateChargePointRequest createChargePointRequest, CancellationToken cancellationToken = default)
    {
        var chargePointToCreate = _mapper.Map<ChargePoint>(createChargePointRequest);
        await _chargePointRepository.AddAsync(chargePointToCreate, cancellationToken);
        await _chargePointRepository.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<ChargePointResponse>(chargePointToCreate);
        return result;
    }

    public async Task<ChargePointResponse> UpdateAsync(UpdateChargePointRequest updateChargePointRequest, CancellationToken cancellationToken = default)
    {
        var chargePointToUpdate = await _chargePointRepository.GetByIdAsync(updateChargePointRequest.Id, cancellationToken);

        if (chargePointToUpdate is null)
            throw new NotFoundException(nameof(ChargePoint), updateChargePointRequest.Id);

        _mapper.Map(updateChargePointRequest, chargePointToUpdate);
        _chargePointRepository.Update(chargePointToUpdate);
        
        await _chargePointRepository.SaveChangesAsync(cancellationToken);
        
        var result = _mapper.Map<ChargePointResponse>(chargePointToUpdate);
        return result;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chargePointToRemove = await _chargePointRepository.GetByIdAsync(id, cancellationToken);

        if (chargePointToRemove == null)
            throw new NotFoundException(nameof(ChargePoint), id);

        _chargePointRepository.Remove(chargePointToRemove);
        
        await _chargePointRepository.SaveChangesAsync(cancellationToken);
    }
}