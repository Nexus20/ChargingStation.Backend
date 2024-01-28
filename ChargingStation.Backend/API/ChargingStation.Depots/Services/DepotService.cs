using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Depots.Models.Responses;
using ChargingStation.Depots.Repositories;
using ChargingStation.Domain.Entities;

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

    public async Task<List<DepotResponse>> GetAsync(CancellationToken cancellationToken = default)
    {
        var depots = await _depotRepository.GetAllAsync(cancellationToken);
        
        if(depots.Count == 0)
            return Enumerable.Empty<DepotResponse>().ToList();
        
        var result = _mapper.Map<List<DepotResponse>>(depots);
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

    public Task<DepotResponse> CreateAsync(DepotResponse depot, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DepotResponse> UpdateAsync(DepotResponse depot, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}