using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models;
using ChargingStation.Depots.Models.Requests;
using ChargingStation.Depots.Models.Responses;
using ChargingStation.Depots.Specifications;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;

namespace ChargingStation.Depots.Services;

public class DepotService : IDepotService
{
    private readonly IRepository<Depot> _depotRepository;
    private readonly IMapper _mapper;

    public DepotService(IRepository<Depot> depotRepository, IMapper mapper)
    {
        _depotRepository = depotRepository;
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
        var depot = await _depotRepository.GetByIdAsync(id, cancellationToken);
        
        if(depot is null)
            throw new NotFoundException(nameof(Depot), id);
        
        var result = _mapper.Map<DepotResponse>(depot);
        return result;
    }

    public async Task<DepotResponse> CreateAsync(DepotResponse depot, CancellationToken cancellationToken = default)
    {
        var existing = await _depotRepository.GetAllAsync(x => x.Name == depot.Name, cancellationToken: cancellationToken);

        if (existing != null)
            throw new BadRequestException(nameof(Depot), depot);

        var createdDepot = _mapper.Map<Depot>(depot);
        await _depotRepository.AddAsync(createdDepot, cancellationToken);

        var result = _mapper.Map<DepotResponse>(createdDepot);
        return result;
    }

    public async Task<DepotResponse> UpdateAsync(DepotResponse depot, CancellationToken cancellationToken = default)
    {
        var depotToUpdate = await _depotRepository.GetByIdAsync(depot.Id, cancellationToken);

        if (depotToUpdate is null)
            throw new NotFoundException(nameof(Depot), depot.Id);

        _mapper.Map(depot, depotToUpdate);
        _depotRepository.Update(depotToUpdate);

        var result = _mapper.Map<DepotResponse>(depotToUpdate);
        return result;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var depotToRemove = await _depotRepository.GetByIdAsync(id, cancellationToken);

        if (depotToRemove == null)
            throw new NotFoundException(nameof(Depot), id);

        _depotRepository.Remove(depotToRemove);
    }
}