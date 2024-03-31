using System.Linq.Expressions;
using ChargingStation.Common.Models;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Abstract;
using ChargingStation.Infrastructure.Extensions;
using ChargingStation.Infrastructure.Persistence;
using ChargingStation.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ChargingStation.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity;
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, 
            IOrderedQueryable<TEntity>>? orderBy = null, string? includeProperties = null, bool isTracking = true, 
            CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = DbSet;

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
        IQueryable<TEntity> query = DbSet;
        return query.ApplySpecifications(specification).ToListAsync(cancellationToken);
    }

    public Task<TEntity?> GetFirstOrDefaultAsync(Specification<TEntity> specification, bool applyTracking = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.ApplySpecifications(specification);
        
        if(!applyTracking)
            query = query.AsNoTracking();
        
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IPagedCollection<TEntity>> GetPagedCollectionAsync(Specification<TEntity> specification, int? pageNumber = 1, int? pageSize = null, bool applyTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = DbSet;
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
        return DbSet.AddAsync(entity, cancellationToken).AsTask();
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }
    
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}