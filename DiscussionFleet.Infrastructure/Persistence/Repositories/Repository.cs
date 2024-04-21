using System.Linq.Expressions;
using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity, TKey>
    : IRepositoryBase<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly DbContext _appDbContext;
    protected readonly DbSet<TEntity> EntityDbSet;

    protected Repository(DbContext context)
    {
        _appDbContext = context;
        EntityDbSet = _appDbContext.Set<TEntity>();
    }

    public virtual async Task CreateAsync(TEntity entity)
    {
        await EntityDbSet.AddAsync(entity).ConfigureAwait(false);
    }

    public async Task CreateManyAsync(ICollection<TEntity> entity)
    {
        await EntityDbSet.AddRangeAsync(entity).ConfigureAwait(false);
    }


    public virtual async Task<TEntity?> GetOneAsync(
        Expression<Func<TEntity, bool>> filter,
        bool useSplitQuery,
        ICollection<Expression<Func<TEntity, object?>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = EntityDbSet.Where(filter);
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();


        return await data.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }


    public virtual async Task<TEntity?> GetOneAsync<TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery,
        bool ascendingOrder = true,
        IList<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    ) where TSorter : IComparable<TSorter>
    {
        var data = EntityDbSet.Where(filter);

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();


        return await data.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }


    public virtual async Task<TResult?> GetOneAsync<TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> subsetSelector,
        bool useSplitQuery,
        IList<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = EntityDbSet.Where(filter);
        if (includes is not null && includes.Count > 0)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();

        return await data.Select(subsetSelector)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }


    public virtual async Task<TResult?> GetOneAsync<TResult, TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> subsetSelector,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery,
        bool ascendingOrder = true,
        IList<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    ) where TSorter : IComparable<TSorter>
    {
        var data = EntityDbSet.Where(filter);

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);


        if (includes is not null && includes.Count > 0)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        return await data.Select(subsetSelector)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }


    public virtual async Task<IList<TEntity>> GetAllAsync(
        bool useSplitQuery,
        uint page = 1, uint limit = 10,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = EntityDbSet.AsQueryable();
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = data.Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(limit)).Take(Convert.ToInt32(limit));

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();
        
        return await data.ToListAsync(cancellationToken).ConfigureAwait(false);
    }


    public virtual async Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter,
        bool useSplitQuery,
        uint page = 1, uint limit = 10,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = EntityDbSet.Where(filter);

        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = data.Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(limit)).Take(Convert.ToInt32(limit));

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();
        
        return await data.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IList<TEntity>> GetAllAsync<TSorter>(
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery,
        uint page = 1,
        uint limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = EntityDbSet.AsQueryable();
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        data = data.Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(limit)).Take(Convert.ToInt32(limit));

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();

        return await data.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IList<TEntity>> GetAllAsync<TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery,
        uint page = 1, uint limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default)
    {
        var data = EntityDbSet.Where(filter);

        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        data = data.Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(limit)).Take(Convert.ToInt32(limit));

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();

        return await data.ToListAsync(cancellationToken).ConfigureAwait(false);
    }


    public virtual async Task<IList<TResult>> GetAllAsync<TResult, TSorter>(
        Expression<Func<TEntity, TResult>> subsetSelector,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery,
        uint page = 1,
        uint limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = EntityDbSet.AsQueryable();
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        data = data.Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(limit)).Take(Convert.ToInt32(limit));

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();

        return await data.Select(subsetSelector)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IList<TResult>> GetAllAsync<TResult, TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> subsetSelector,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery,
        uint page = 1,
        uint limit = 10, bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default)
    {
        var data = EntityDbSet.Where(filter);
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        data = data.Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(limit)).Take(Convert.ToInt32(limit));

        if (useSplitQuery) data = data.AsSplitQuery();

        if (disableTracking) data = data.AsNoTracking();

        return await data.Select(subsetSelector)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }


    public virtual Task UpdateAsync(TEntity entityToUpdate)
    {
        return Task.Run(() =>
        {
            EntityDbSet.Attach(entityToUpdate);
            _appDbContext.Entry(entityToUpdate).State = EntityState.Modified;
        });
    }


    public virtual Task RemoveAsync(TEntity entityToDelete)
    {
        return Task.Run(() =>
        {
            if (_appDbContext.Entry(entityToDelete).State is EntityState.Detached)
            {
                EntityDbSet.Attach(entityToDelete);
            }

            EntityDbSet.Remove(entityToDelete);
        });
    }

    public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        int count;

        if (filter is not null)
        {
            count = await EntityDbSet
                .CountAsync(filter, cancellationToken)
                .ConfigureAwait(false);
        }

        else
        {
            count = await EntityDbSet.CountAsync(cancellationToken).ConfigureAwait(false);
        }

        return count;
    }


    public Task TrackEntityAsync<T>(T entity) where T : class
    {
        return Task.Run(() => _appDbContext.Set<T>().Attach(entity));
    }

    public Task TrackEntityAsync(TEntity entity)
    {
        return Task.Run(() => _appDbContext.Set<TEntity>().Attach(entity));
    }

    public Task ModifyEntityStateToAddedAsync(TEntity entity)
    {
        return Task.Run(() =>
        {
            if (_appDbContext.Entry(entity).State is not EntityState.Added)
            {
                _appDbContext.Entry(entity).State = EntityState.Added;
            }
        });
    }


    public Task ModifyEntityStateToAddedAsync<T>(T entity)
    {
        return Task.Run(() =>
        {
            if (entity is null) return;
            if (_appDbContext.Entry(entity).State is not EntityState.Added)
            {
                _appDbContext.Entry(entity).State = EntityState.Added;
            }
        });
    }
}