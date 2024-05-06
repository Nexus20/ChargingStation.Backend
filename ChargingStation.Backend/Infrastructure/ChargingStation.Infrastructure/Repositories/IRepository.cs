using System.Linq.Expressions;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Abstract;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.Infrastructure.Repositories;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>>? orderBy = null, string? includeProperties = null, bool isTracking = true,
            CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<TEntity?> GetFirstOrDefaultAsync(Specification<TEntity> specification, bool applyTracking = false, CancellationToken cancellationToken = default);

    Task<IPagedCollection<TEntity>> GetPagedCollectionAsync(Specification<TEntity> specification, int? pageNumber = 1, int? pageSize = null, bool applyTracking = false, CancellationToken cancellationToken = default);
    
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}