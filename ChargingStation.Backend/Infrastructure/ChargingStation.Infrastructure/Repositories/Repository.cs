using System.Linq.Expressions;
using ChargingStation.Common.Models;
using ChargingStation.Domain.Abstract;
using ChargingStation.Infrastructure.Extensions;
using ChargingStation.Infrastructure.Persistence;
using ChargingStation.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ChargingStation.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity;
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, 
            IOrderedQueryable<TEntity>>? orderBy = null, string? includeProperties = null, bool isTracking = true, 
            CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        if (orderBy != null)
            query = orderBy(query);

        if (!isTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        return query.ApplySpecifications(specification).ToListAsync(cancellationToken);
    }

    public Task<TEntity?> GetFirstOrDefaultAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        return query.ApplySpecifications(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IPagedCollection<TEntity>> GetPagedCollectionAsync(Specification<TEntity> specification, int? pageNumber = 1, int? pageSize = null, bool applyTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        var totalCollectionCount = await query.ApplySpecifications(specification).CountAsync(cancellationToken: cancellationToken);
        
        var page = pageNumber ?? 1;
        var size = pageSize ?? totalCollectionCount;
        
        query = query.ApplySpecifications(specification)
            .Skip((page - 1) * size)
            .Take(size);
        
        if(!applyTracking)
            query = query.AsNoTracking();
            
        var data = await query.ToListAsync(cancellationToken: cancellationToken);
        
        return new PagedCollection<TEntity>(data, totalCollectionCount, size, page);
    }


    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return _dbSet.AddAsync(entity, cancellationToken).AsTask();
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }
    
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}