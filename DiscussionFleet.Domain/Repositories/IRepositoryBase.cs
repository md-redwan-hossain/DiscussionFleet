using System.Linq.Expressions;
using DiscussionFleet.Domain.Entities.Contracts;

namespace DiscussionFleet.Domain.Repositories;

public interface IRepositoryBase<TEntity, in TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    Task CreateAsync(TEntity entity);
    Task CreateManyAsync(ICollection<TEntity> entity);

    Task<TEntity?> GetOneAsync(
        Expression<Func<TEntity, bool>> filter,
        ICollection<Expression<Func<TEntity, object?>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<TEntity?> GetOneAsync<TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool ascendingOrder = true,
        IList<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    ) where TSorter : IComparable<TSorter>;

    Task<TResult?> GetOneAsync<TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> subsetSelector,
        IList<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<TResult?> GetOneAsync<TResult, TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> subsetSelector,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool ascendingOrder = true,
        IList<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    ) where TSorter : IComparable<TSorter>;


    Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter,
        uint page = 1, uint limit = 10,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    );


    Task<IList<TEntity>> GetAllAsync(
        uint page = 1, uint limit = 10,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<IList<TEntity>> GetAllAsync<TSorter>(
        Expression<Func<TEntity, TSorter>> orderBy,
        uint page = 1,
        uint limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<IList<TEntity>> GetAllAsync<TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TSorter>> orderBy,
        uint page = 1,
        uint limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<IList<TResult>> GetAllAsync<TResult, TSorter>(
        Expression<Func<TEntity, TResult>> subsetSelector,
        Expression<Func<TEntity, TSorter>> orderBy,
        uint page = 1,
        uint limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    );


    Task<IList<TResult>> GetAllAsync<TResult, TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> subsetSelector,
        Expression<Func<TEntity, TSorter>> orderBy,
        uint page = 1,
        uint limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(TEntity entityToUpdate);

    Task RemoveAsync(TEntity entityToDelete);

    Task<int> GetCountAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default
    );

    Task ModifyEntityStateToAddedAsync<T>(T entity);
    Task ModifyEntityStateToAddedAsync(TEntity entity);
    Task TrackEntityAsync<T>(T entity) where T : class;
    Task TrackEntityAsync(TEntity entity);
}