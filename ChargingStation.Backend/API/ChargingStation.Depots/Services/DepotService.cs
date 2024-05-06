using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.TimeZone;
using ChargingStation.Depots.Models.Requests;
using ChargingStation.Depots.Specifications;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using TimeZone = ChargingStation.Domain.Entities.TimeZone;

namespace ChargingStation.Depots.Services;

public class DepotService : IDepotService
{
    private readonly IRepository<Depot> _depotRepository;
    private readonly IRepository<TimeZone> _timeZoneRepository;
    private readonly IMapper _mapper;

    public DepotService(IRepository<Depot> depotRepository, IRepository<TimeZone> timeZoneRepository, IMapper mapper)
    {
        _depotRepository = depotRepository;
        _timeZoneRepository = timeZoneRepository;
        _mapper = mapper;
    }

    public async Task<IPagedCollection<DepotResponse>> GetAsync(GetDepotsRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetDepotsSpecification(request);
        
        var depots = await _depotRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);
        
        if(!depots.Collection.Any())
            return PagedCollection<DepotResponse>.Empty;
        
        var result = _mapper.Map<IPagedCollection<DepotResponse>>(depots);
        return result;
    }

    public async Task<DepotResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var specification = new GetDepotSpecification(id);

        var depot = await _depotRepository.GetAsync(specification, cancellationToken);
        
        if(depot is null)
            throw new NotFoundException(nameof(Depot), id);
        
        var result = _mapper.Map<DepotResponse>(depot);
        return result;
    }

    public async Task<DepotResponse> CreateAsync(CreateDepotRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetDepotsSpecification(new GetDepotsRequest()
        {
            Name = request.Name 
        });
        
        var existing = await _depotRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);

        if (existing is not null)
            throw new BadRequestException(nameof(Depot), request);

        var createdDepot = _mapper.Map<Depot>(request);
        await _depotRepository.AddAsync(createdDepot, cancellationToken);
        await _depotRepository.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<DepotResponse>(createdDepot);
        return result;
    }

    public async Task<DepotResponse> UpdateAsync(UpdateDepotRequest request, CancellationToken cancellationToken = default)
    {
        var depotToUpdate = await _depotRepository.GetByIdAsync(request.Id, cancellationToken);

        if (depotToUpdate is null)
            throw new NotFoundException(nameof(Depot), request.Id);

        _mapper.Map(request, depotToUpdate);
        _depotRepository.Update(depotToUpdate);
        await _depotRepository.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<DepotResponse>(depotToUpdate);
        return result;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var depotToRemove = await _depotRepository.GetByIdAsync(id, cancellationToken);

        if (depotToRemove == null)
            throw new NotFoundException(nameof(Depot), id);

        _depotRepository.Remove(depotToRemove);
        await _depotRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IPagedCollection<TimeZoneResponse>> GetTimeZonesAsync(GetTimeZoneRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetTimeZonesSpecification(request);

        var timeZones = await _timeZoneRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);

        if (!timeZones.Collection.Any())
            return PagedCollection<TimeZoneResponse>.Empty;

        var result = _mapper.Map<IPagedCollection<TimeZoneResponse>>(timeZones);
        return result;
    }
}