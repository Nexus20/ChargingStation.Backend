using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.OcppTags.Responses;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using OcppTags.Application.Models.Requests;
using OcppTags.Application.Specifications;

namespace OcppTags.Application.Services;

public class OcppTagService : IOcppTagService
{
    private readonly IRepository<OcppTag> _ocppTagRepository;
    private readonly IMapper _mapper;

    public OcppTagService(IRepository<OcppTag> ocppTagRepository, IMapper mapper)
    {
        _ocppTagRepository = ocppTagRepository;
        _mapper = mapper;
    }

    public async Task<IPagedCollection<OcppTagResponse>> GetAsync(GetOcppTagsRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetOcppTagsSpecification(request);
        
        var ocppTags = await _ocppTagRepository.GetPagedCollectionAsync(specification, request.PagePredicate?.Page, request.PagePredicate?.PageSize, cancellationToken: cancellationToken);
        
        if (!ocppTags.Collection.Any())
            return PagedCollection<OcppTagResponse>.Empty;
        
        var result = _mapper.Map<IPagedCollection<OcppTagResponse>>(ocppTags);
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

    public async Task<OcppTagResponse?> GetByOcppTagIdAsync(string ocppTagId, CancellationToken cancellationToken = default)
    {
        var request = new GetOcppTagsRequest
        {
            TagId = ocppTagId
        };
        
        var specification = new GetOcppTagsSpecification(request);
        
        var ocppTag = await _ocppTagRepository.GetFirstOrDefaultAsync(specification, cancellationToken: cancellationToken);

        if (ocppTag is null)
            return null; 
        
        var result = _mapper.Map<OcppTagResponse>(ocppTag);
        return result;
    }

    public async Task<OcppTagResponse> CreateAsync(CreateOcppTagRequest request, string? userId, CancellationToken cancellationToken = default)
    {
        if (userId == null)
        {
            throw new BadRequestException("User ID not found in the token.");
        }

        var ocppTagToCreate = _mapper.Map<OcppTag>(request);
        ocppTagToCreate.ApplicationUserId = Guid.Parse(userId);
        
        await _ocppTagRepository.AddAsync(ocppTagToCreate, cancellationToken);
        await _ocppTagRepository.SaveChangesAsync(cancellationToken);
        
        var result = _mapper.Map<OcppTagResponse>(ocppTagToCreate);
        return result;
    }

    public async Task<OcppTagResponse> UpdateAsync(UpdateOcppTagRequest request, CancellationToken cancellationToken = default)
    {
        var ocppTagToUpdate = await _ocppTagRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (ocppTagToUpdate is null)
            throw new NotFoundException(nameof(OcppTag), request.Id);
        
        _mapper.Map(request, ocppTagToUpdate);
        _ocppTagRepository.Update(ocppTagToUpdate);
        await _ocppTagRepository.SaveChangesAsync(cancellationToken);
        
        var result = _mapper.Map<OcppTagResponse>(ocppTagToUpdate);
        return result;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ocppTagToRemove = await _ocppTagRepository.GetByIdAsync(id, cancellationToken);
        
        if (ocppTagToRemove is null)
            throw new NotFoundException(nameof(OcppTag), id);
        
        _ocppTagRepository.Remove(ocppTagToRemove);
        await _ocppTagRepository.SaveChangesAsync(cancellationToken);
    }
}