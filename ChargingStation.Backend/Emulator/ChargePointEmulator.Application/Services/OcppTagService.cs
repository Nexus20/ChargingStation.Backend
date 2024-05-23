using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models.OcppTags.Responses;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;

namespace ChargePointEmulator.Application.Services;

public class OcppTagService : IOcppTagService
{
    private readonly IRepository<OcppTag> _ocppTagRepository;
    private readonly IMapper _mapper;

    public OcppTagService(IRepository<OcppTag> ocppTagRepository, IMapper mapper)
    {
        _ocppTagRepository = ocppTagRepository;
        _mapper = mapper;
    }

    public async Task<List<OcppTagResponse>> GetAsync(CancellationToken cancellationToken = default)
    {
        var ocppTags = await _ocppTagRepository.GetAllAsync(cancellationToken);
        
        if (!ocppTags.Any())
            return [];
        
        var result = _mapper.Map<List<OcppTagResponse>>(ocppTags);
        return result;
    }

    public async Task<OcppTagResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ocppTag = await _ocppTagRepository.GetByIdAsync(id, cancellationToken);
        
        if (ocppTag is null)
            throw new NotFoundException(nameof(OcppTag), id);
        
        var result = _mapper.Map<OcppTagResponse>(ocppTag);
        return result;
    }
}