using System.Linq.Expressions;
using DiscussionFleet.Domain.Entities.Contracts;

namespace DiscussionFleet.Domain.Repositories;

public interface IRepositoryBase<TEntity, in TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    Task CreateAsync(TEntity entity);
    Task CreateAsync(IEnumerable<TEntity> entities);

    Task<TEntity?> GetOneAsync(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, object>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<TResult?> GetOneAsync<TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, object>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, object>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(TEntity entityToUpdate);
    Task UpdateAsync(IEnumerable<TEntity> entitiesToUpdate);
    Task<bool> RemoveAsync(TKey id);
    Task RemoveAsync(TEntity entityToDelete);
    Task RemoveAsync(IEnumerable<TEntity>  entitiesToDelete);

    Task<int> GetCountAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default
    );
}